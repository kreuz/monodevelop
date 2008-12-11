//
// ProjectCodeCompletionDatabase.cs
//
// Author:
//   Lluis Sanchez Gual
//
// Copyright (C) 2005 Novell, Inc (http://www.novell.com)
//
// Permission is hereby granted, free of charge, to any person obtaining
// a copy of this software and associated documentation files (the
// "Software"), to deal in the Software without restriction, including
// without limitation the rights to use, copy, modify, merge, publish,
// distribute, sublicense, and/or sell copies of the Software, and to
// permit persons to whom the Software is furnished to do so, subject to
// the following conditions:
// 
// The above copyright notice and this permission notice shall be
// included in all copies or substantial portions of the Software.
// 
// THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND,
// EXPRESS OR IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF
// MERCHANTABILITY, FITNESS FOR A PARTICULAR PURPOSE AND
// NONINFRINGEMENT. IN NO EVENT SHALL THE AUTHORS OR COPYRIGHT HOLDERS BE
// LIABLE FOR ANY CLAIM, DAMAGES OR OTHER LIABILITY, WHETHER IN AN ACTION
// OF CONTRACT, TORT OR OTHERWISE, ARISING FROM, OUT OF OR IN CONNECTION
// WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN THE SOFTWARE.
//

using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;

using MonoDevelop.Core;
using MonoDevelop.Projects;
using MonoDevelop.Projects.Dom.Parser;
using System.Reflection;

namespace MonoDevelop.Projects.Dom.Serialization
{
	internal class ProjectCodeCompletionDatabase : SerializationCodeCompletionDatabase
	{
		Project project;
		bool initialFileCheck;
		string lastVersion;
		int parseCount;
		
		public ProjectCodeCompletionDatabase (Project project, ParserDatabase pdb): base (pdb)
		{
			this.project = project;
			SetLocation (project.BaseDirectory, project.Name);
			
			Read ();
			
			UpdateFromProject ();
			
			project.FileChangedInProject   += new ProjectFileEventHandler (OnFileChanged);
			project.FileAddedToProject     += new ProjectFileEventHandler (OnFileAdded);
			project.FileRemovedFromProject += new ProjectFileEventHandler (OnFileRemoved);
			project.FileRenamedInProject   += new ProjectFileRenamedEventHandler (OnFileRenamed);
			project.Modified               += new SolutionItemModifiedEventHandler (OnProjectModified);

			initialFileCheck = true;
		}
		
		public Project Project {
			get { return project; }
		}
		
		public override void Dispose ()
		{
			base.Dispose ();
			project.FileChangedInProject -= new ProjectFileEventHandler (OnFileChanged);
			project.FileAddedToProject -= new ProjectFileEventHandler (OnFileAdded);
			project.FileRemovedFromProject -= new ProjectFileEventHandler (OnFileRemoved);
			project.FileRenamedInProject -= new ProjectFileRenamedEventHandler (OnFileRenamed);
			project.Modified -= new SolutionItemModifiedEventHandler (OnProjectModified);
		}
		
		public override void CheckModifiedFiles ()
		{
			// Once the first modification check is done, change detection
			// is done through project events
			
			if (initialFileCheck) {
				base.CheckModifiedFiles ();
				initialFileCheck = false;
			}
		}
		
		void OnFileChanged (object sender, ProjectFileEventArgs args)
		{
			FileEntry file = GetFile (args.ProjectFile.Name);
			if (file != null) QueueParseJob (file);
		}
		
		void OnFileAdded (object sender, ProjectFileEventArgs args)
		{
			if (args.ProjectFile.BuildAction == BuildAction.Compile) {
				FileEntry file = AddFile (args.ProjectFile.Name);
				// CheckModifiedFiles won't detect new files, so parsing
				// must be manyally signaled
				QueueParseJob (file);
			}
		}

		void OnFileRemoved (object sender, ProjectFileEventArgs args)
		{
			RemoveFile (args.ProjectFile.Name);
		}

		void OnFileRenamed (object sender, ProjectFileRenamedEventArgs args)
		{
			if (args.ProjectFile.BuildAction == BuildAction.Compile) {
				RemoveFile (args.OldName);
				FileEntry file = AddFile (args.NewName);
				// CheckModifiedFiles won't detect new files, so parsing
				// must be manyally signaled
				QueueParseJob (file);
			}
		}
		
		void OnProjectModified (object s, SolutionItemModifiedEventArgs args)
		{
			UpdateCorlibReference ();
			if (UpdateCorlibReference ())
				SourceProjectDom.UpdateReferences ();
		}

		public void UpdateFromProject ()
		{
			Hashtable fs = new Hashtable ();
			foreach (ProjectFile file in project.Files)
			{
				if (file.BuildAction != BuildAction.Compile) continue;
				if (GetFile (file.Name) == null) AddFile (file.Name);
				fs [file.Name] = null;
			}
			
			ArrayList keys = new ArrayList ();
			keys.AddRange (files.Keys);
			foreach (string file in keys)
			{
				if (!fs.Contains (file))
					RemoveFile (file);
			}
			
			fs.Clear ();
			if (project is DotNetProject) {
				DotNetProject netProject = (DotNetProject) project;
				foreach (ProjectReference pr in netProject.References)
				{
					string[] refIds = GetReferenceKeys (pr);
					foreach (string refId in refIds) {
						fs[refId] = null;
						if (!HasReference (refId))
							AddReference (refId);
					}
				}
			}
			keys.Clear();
			keys.AddRange (references);
			foreach (ReferenceEntry re in keys)
			{
				// Don't delete corlib references. They are implicit to projects, but not to pidbs.
				if (!fs.Contains (re.Uri) && !IsCorlibReference (re))
					RemoveReference (re.Uri);
			}
			UpdateCorlibReference ();
		}
		
		bool UpdateCorlibReference ()
		{
			// Creates a reference to the correct version of mscorlib, depending
			// on the target runtime version. Returns true if the references
			// have changed.
			
			DotNetProject prj = project as DotNetProject;
			if (prj == null) return false;
			
			if (prj.TargetFramework.Id == lastVersion)
				return false;

			// Look for an existing mscorlib reference
			string currentRefUri = null;
			foreach (ReferenceEntry re in References) {
				if (IsCorlibReference (re)) {
					currentRefUri = re.Uri;
					break;
				}
			}
			
			// Gets the name and version of the mscorlib assembly required by the project
			string requiredRefUri = "Assembly:";
			requiredRefUri += Runtime.SystemAssemblyService.GetAssemblyNameForVersion (typeof(object).Assembly.GetName().ToString(), prj.TargetFramework);
			
			// Replace the old reference if the target version has changed
			if (currentRefUri != null) {
				if (currentRefUri != requiredRefUri) {
					RemoveReference (currentRefUri);
					AddReference (requiredRefUri);
					return true;
				}
			} else {
				AddReference (requiredRefUri);
				return true;
			}
			return false;
		}
		
		bool IsCorlibReference (ReferenceEntry re)
		{
			return re.Uri.StartsWith ("Assembly:mscorlib");
		}
		
		string[] GetReferenceKeys (ProjectReference pr)
		{
			switch (pr.ReferenceType) {
			case ReferenceType.Project:
				Project referencedProject = project.ParentSolution.FindProjectByName (pr.Reference);
				return new string[] { "Project:" + (referencedProject != null ? referencedProject.FileName : "null") };
			case ReferenceType.Gac:
				string refId = pr.Reference;
				string ext = Path.GetExtension (refId).ToLower ();
				if (ext == ".dll" || ext == ".exe")
					refId = refId.Substring (0, refId.Length - 4);
				return new string[] { "Assembly:" + refId };
			case ReferenceType.Assembly:
				return new string[] { "Assembly:" + pr.Reference };
			default:
				ArrayList list = new ArrayList ();
				foreach (string s in pr.GetReferencedFileNames (ProjectService.DefaultConfiguration))
					list.Add ("Assembly:" + s);
				return (string[]) list.ToArray (typeof(string));
			}
		}
		
		protected override void ParseFile (string fileName, IProgressMonitor monitor)
		{
			if (monitor != null) monitor.BeginTask ("Parsing file: " + Path.GetFileName (fileName), 1);
			
			try {
				ParsedDocument parserInfo = ProjectDomService.Parse (this.project,
				                                                         fileName,
				                                                         null,
				                                                         delegate () { return File.ReadAllText (fileName); });
				if (parserInfo != null) {
					lock (rwlock) {
						TypeUpdateInformation res = UpdateFromParseInfo (parserInfo.CompilationUnit, fileName);
						if (res != null) 
							ProjectDomService.NotifyTypeUpdate (project, fileName, res);
					}
				}
			} finally {
				if (monitor != null) monitor.EndTask ();
			}
		}
		
		public TypeUpdateInformation UpdateFromParseInfo (ICompilationUnit parserInfo, string fileName)
		{
			ICompilationUnit cu = parserInfo;

			List<IType> resolved;
			
			bool allResolved = ResolveTypes (cu, cu.Types, out resolved);
			TypeUpdateInformation res = UpdateTypeInformation (resolved, parserInfo.FileName);
			
			FileEntry file = files [fileName] as FileEntry;
			if (file != null) {
				// TODO: TagComments
//				if (file.CommentTasks != parserInfo.TagComments)
//				{
//					file.CommentTasks = parserInfo.TagComments;
//					parserDatabase.UpdatedCommentTasks (file);
//				}
				
				
				if (!allResolved) {
					if (file.ParseErrorRetries > 0) {
						file.ParseErrorRetries--;
					}
					else
						file.ParseErrorRetries = 3;
				}
				else
					file.ParseErrorRetries = 0;
			}
			
			if ((++parseCount % MAX_ACTIVE_COUNT) == 0)
				Flush ();
			return res;
		}
		
		protected override void OnFileRemoved (string fileName, TypeUpdateInformation classInfo)
		{
			if (classInfo.Removed.Count > 0)
				ProjectDomService.NotifyTypeUpdate (project, fileName, classInfo);
		}

	}
}
