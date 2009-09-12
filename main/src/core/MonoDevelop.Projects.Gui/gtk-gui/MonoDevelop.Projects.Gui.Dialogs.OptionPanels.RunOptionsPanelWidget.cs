// ------------------------------------------------------------------------------
//  <autogenerated>
//      This code was generated by a tool.
//      
// 
//      Changes to this file may cause incorrect behavior and will be lost if 
//      the code is regenerated.
//  </autogenerated>
// ------------------------------------------------------------------------------

namespace MonoDevelop.Projects.Gui.Dialogs.OptionPanels {
    
    
    public partial class RunOptionsPanelWidget {
        
        private Gtk.VBox vbox67;
        
        private Gtk.VBox vbox69;
        
        private Gtk.Table table10;
        
        private Gtk.Label label100;
        
        private Gtk.Entry parametersEntry;
        
        private Gtk.CheckButton externalConsoleCheckButton;
        
        private Gtk.CheckButton pauseConsoleOutputCheckButton;
        
        private Gtk.HSeparator hseparator1;
        
        private Gtk.Label label1;
        
        private MonoDevelop.Projects.Gui.Dialogs.OptionPanels.EnvVarList envVarList;
        
        protected virtual void Build() {
            Stetic.Gui.Initialize(this);
            // Widget MonoDevelop.Projects.Gui.Dialogs.OptionPanels.RunOptionsPanelWidget
            Stetic.BinContainer.Attach(this);
            this.Name = "MonoDevelop.Projects.Gui.Dialogs.OptionPanels.RunOptionsPanelWidget";
            // Container child MonoDevelop.Projects.Gui.Dialogs.OptionPanels.RunOptionsPanelWidget.Gtk.Container+ContainerChild
            this.vbox67 = new Gtk.VBox();
            this.vbox67.Name = "vbox67";
            this.vbox67.Spacing = 6;
            // Container child vbox67.Gtk.Box+BoxChild
            this.vbox69 = new Gtk.VBox();
            this.vbox69.Name = "vbox69";
            this.vbox69.Spacing = 6;
            // Container child vbox69.Gtk.Box+BoxChild
            this.table10 = new Gtk.Table(((uint)(1)), ((uint)(2)), false);
            this.table10.Name = "table10";
            this.table10.ColumnSpacing = ((uint)(6));
            // Container child table10.Gtk.Table+TableChild
            this.label100 = new Gtk.Label();
            this.label100.Name = "label100";
            this.label100.Xalign = 0F;
            this.label100.LabelProp = MonoDevelop.Core.GettextCatalog.GetString("Paramet_ers:");
            this.label100.UseUnderline = true;
            this.table10.Add(this.label100);
            Gtk.Table.TableChild w1 = ((Gtk.Table.TableChild)(this.table10[this.label100]));
            w1.XOptions = ((Gtk.AttachOptions)(4));
            w1.YOptions = ((Gtk.AttachOptions)(0));
            // Container child table10.Gtk.Table+TableChild
            this.parametersEntry = new Gtk.Entry();
            this.parametersEntry.Name = "parametersEntry";
            this.parametersEntry.IsEditable = true;
            this.parametersEntry.InvisibleChar = '●';
            this.table10.Add(this.parametersEntry);
            Gtk.Table.TableChild w2 = ((Gtk.Table.TableChild)(this.table10[this.parametersEntry]));
            w2.LeftAttach = ((uint)(1));
            w2.RightAttach = ((uint)(2));
            w2.YOptions = ((Gtk.AttachOptions)(0));
            this.vbox69.Add(this.table10);
            Gtk.Box.BoxChild w3 = ((Gtk.Box.BoxChild)(this.vbox69[this.table10]));
            w3.Position = 0;
            w3.Expand = false;
            w3.Fill = false;
            // Container child vbox69.Gtk.Box+BoxChild
            this.externalConsoleCheckButton = new Gtk.CheckButton();
            this.externalConsoleCheckButton.Name = "externalConsoleCheckButton";
            this.externalConsoleCheckButton.Label = MonoDevelop.Core.GettextCatalog.GetString("Run on e_xternal console");
            this.externalConsoleCheckButton.DrawIndicator = true;
            this.externalConsoleCheckButton.UseUnderline = true;
            this.vbox69.Add(this.externalConsoleCheckButton);
            Gtk.Box.BoxChild w4 = ((Gtk.Box.BoxChild)(this.vbox69[this.externalConsoleCheckButton]));
            w4.Position = 1;
            w4.Expand = false;
            w4.Fill = false;
            // Container child vbox69.Gtk.Box+BoxChild
            this.pauseConsoleOutputCheckButton = new Gtk.CheckButton();
            this.pauseConsoleOutputCheckButton.Name = "pauseConsoleOutputCheckButton";
            this.pauseConsoleOutputCheckButton.Label = MonoDevelop.Core.GettextCatalog.GetString("Pause _console output");
            this.pauseConsoleOutputCheckButton.DrawIndicator = true;
            this.pauseConsoleOutputCheckButton.UseUnderline = true;
            this.vbox69.Add(this.pauseConsoleOutputCheckButton);
            Gtk.Box.BoxChild w5 = ((Gtk.Box.BoxChild)(this.vbox69[this.pauseConsoleOutputCheckButton]));
            w5.Position = 2;
            w5.Expand = false;
            w5.Fill = false;
            // Container child vbox69.Gtk.Box+BoxChild
            this.hseparator1 = new Gtk.HSeparator();
            this.hseparator1.Name = "hseparator1";
            this.vbox69.Add(this.hseparator1);
            Gtk.Box.BoxChild w6 = ((Gtk.Box.BoxChild)(this.vbox69[this.hseparator1]));
            w6.Position = 3;
            w6.Expand = false;
            w6.Fill = false;
            // Container child vbox69.Gtk.Box+BoxChild
            this.label1 = new Gtk.Label();
            this.label1.Name = "label1";
            this.label1.Xalign = 0F;
            this.label1.LabelProp = MonoDevelop.Core.GettextCatalog.GetString("Environment Variables:");
            this.vbox69.Add(this.label1);
            Gtk.Box.BoxChild w7 = ((Gtk.Box.BoxChild)(this.vbox69[this.label1]));
            w7.Position = 4;
            w7.Expand = false;
            w7.Fill = false;
            // Container child vbox69.Gtk.Box+BoxChild
            this.envVarList = null;
            this.vbox69.Add(this.envVarList);
            Gtk.Box.BoxChild w8 = ((Gtk.Box.BoxChild)(this.vbox69[this.envVarList]));
            w8.Position = 5;
            this.vbox67.Add(this.vbox69);
            Gtk.Box.BoxChild w9 = ((Gtk.Box.BoxChild)(this.vbox67[this.vbox69]));
            w9.Position = 0;
            this.Add(this.vbox67);
            if ((this.Child != null)) {
                this.Child.ShowAll();
            }
            this.label100.MnemonicWidget = this.parametersEntry;
            this.Show();
        }
    }
}
