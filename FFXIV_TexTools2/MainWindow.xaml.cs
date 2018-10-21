﻿// FFXIV TexTools
// Copyright © 2017 Rafael Gonzalez - All Rights Reserved
// 
// This program is free software: you can redistribute it and/or modify
// it under the terms of the GNU General Public License as published by
// the Free Software Foundation, either version 3 of the License, or
// (at your option) any later version.
// 
// This program is distributed in the hope that it will be useful,
// but WITHOUT ANY WARRANTY; without even the implied warranty of
// MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
// GNU General Public License for more details.
// 
// You should have received a copy of the GNU General Public License
// along with this program.  If not, see <http://www.gnu.org/licenses/>.

using FFXIV_TexTools2.Helpers;
using FFXIV_TexTools2.Model;
using FFXIV_TexTools2.Resources;
using FFXIV_TexTools2.ViewModel;
using FFXIV_TexTools2.Views;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.IO;
using System.Windows;
using System.Windows.Forms;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using Application = System.Windows.Application;

namespace FFXIV_TexTools2
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        MainViewModel mViewModel;
        CategoryViewModel selectedItem;



        public MainWindow()
        {
            InitializeComponent();
            mViewModel = new MainViewModel();
            this.DataContext = mViewModel;

            var dxver = Properties.Settings.Default.DX_Ver;

            if (dxver != Strings.DX11 && dxver != Strings.DX9)
            {
                Properties.Settings.Default.DX_Ver = Strings.DX11;
                Properties.Settings.Default.Save();
            }

            DXVerButton.Content = "DX Version: " + dxver.Substring(2);


            //HavokInterop.InitializeSTA();
        }

        private void Menu_ProblemCheck_Click(object sender, RoutedEventArgs e)
        {
            ProblemCheckView pcv = new ProblemCheckView();
            pcv.Owner = this;
            pcv.Show();
        }

        private void Menu_BugReport_Click(object sender, RoutedEventArgs e)
        {
            System.Diagnostics.Process.Start("https://bitbucket.org/liinko/ffxiv-textools/issues");
        }

        private void Menu_About_Click(object sender, RoutedEventArgs e)
        {
            About a = new About();
            a.Owner = GetWindow(this);
            a.Show();
        }

        private void Menu_English_Click(object sender, RoutedEventArgs e)
        {
            if (FlexibleMessageBox.Show("Changing language requires the application to restart. \nRestart now?", "Language Change",MessageBoxButtons.OKCancel,MessageBoxIcon.Information) == System.Windows.Forms.DialogResult.OK)
            {
                Properties.Settings.Default.Language = "en";
                Properties.Settings.Default.Save();

                System.Windows.Forms.Application.Restart();
                Application.Current.Shutdown();
            }
            else
            {
                Menu_English.IsChecked = false;
            }
        }

        private void Menu_Japanese_Click(object sender, RoutedEventArgs e)
        {


            if (FlexibleMessageBox.Show("Changing language requires the application to restart. \nRestart now?", "Language Change",MessageBoxButtons.OKCancel,MessageBoxIcon.Information) == System.Windows.Forms.DialogResult.OK)
            {
                Properties.Settings.Default.Language = "ja";
                Properties.Settings.Default.Save();

                System.Windows.Forms.Application.Restart();
                Application.Current.Shutdown();
            }
            else
            {
                Menu_Japanese.IsChecked = false;
            }
        }

        private void Menu_French_Click(object sender, RoutedEventArgs e)
        {
            if (FlexibleMessageBox.Show("Changing language requires the application to restart. \nRestart now?", "Language Change",MessageBoxButtons.OKCancel,MessageBoxIcon.Information) == System.Windows.Forms.DialogResult.OK)
            {
                Properties.Settings.Default.Language = "fr";
                Properties.Settings.Default.Save();

                System.Windows.Forms.Application.Restart();
                Application.Current.Shutdown();
            }
            else
            {
                Menu_French.IsChecked = false;
            }
        }

        private void Menu_German_Click(object sender, RoutedEventArgs e)
        {
            if (FlexibleMessageBox.Show("Changing language requires the application to restart. \nRestart now?", "Language Change",MessageBoxButtons.OKCancel,MessageBoxIcon.Information) == System.Windows.Forms.DialogResult.OK)
            {
                Properties.Settings.Default.Language = "de";
                Properties.Settings.Default.Save();

                System.Windows.Forms.Application.Restart();
                Application.Current.Shutdown();
            }
            else
            {
                Menu_German.IsChecked = false;
            }
        }

        private void Menu_ModList_Click(object sender, RoutedEventArgs e)
        {
            
            ModList ml = new ModList(this.mViewModel.ModelVM, this.mViewModel.TextureVM);
            ml.Owner = this;
            ml.Show();
        }

        private void Menu_Directories_Click(object sender, RoutedEventArgs e)
        {
            DirectoriesView dv = new DirectoriesView();
            dv.Owner = this;
            dv.Show();
        }

        private void Menu_RevertAll_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                RevertAll();
            }
            catch (Exception ex)
            {
                FlexibleMessageBox.Show("Error Accessing .modlist File \n" + ex.Message, "MainWindow Error " + Info.appVersion, MessageBoxButtons.OK, MessageBoxIcon.Error);
            }

        }

        private void RevertAll()
        {
            JsonEntry modEntry = null;
            string line;

            using (StreamReader sr = new StreamReader(Properties.Settings.Default.Modlist_Directory))
            {
                while ((line = sr.ReadLine()) != null)
                {
                    modEntry = JsonConvert.DeserializeObject<JsonEntry>(line);
                    if (modEntry.originalOffset != 0)
                    {
                        Helper.UpdateIndex(modEntry.originalOffset, modEntry.fullPath, modEntry.datFile);
                        Helper.UpdateIndex2(modEntry.originalOffset, modEntry.fullPath, modEntry.datFile);
                    }
                }
            }
        }

        private void Menu_ReapplyAll_Click(object sender, RoutedEventArgs e)
        {
            JsonEntry modEntry = null;
            string line;
            try
            {
                using (StreamReader sr = new StreamReader(Properties.Settings.Default.Modlist_Directory))
                {
                    while ((line = sr.ReadLine()) != null)
                    {
                        modEntry = JsonConvert.DeserializeObject<JsonEntry>(line);
                        if(modEntry.originalOffset != 0)
                        {
                            Helper.UpdateIndex(modEntry.modOffset, modEntry.fullPath, modEntry.datFile);
                            Helper.UpdateIndex2(modEntry.modOffset, modEntry.fullPath, modEntry.datFile);
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                FlexibleMessageBox.Show("Error Accessing .modlist File \n" + ex.Message, "MainWindow Error " + Info.appVersion, MessageBoxButtons.OK, MessageBoxIcon.Error);
            }

        }

        private void Window_Closed(object sender, EventArgs e)
        {
            App.Current.Shutdown();
        }

        private void textureTreeView_SelectedItemChanged(object sender, RoutedPropertyChangedEventArgs<object> e)
        {
            var item = e.NewValue as CategoryViewModel;
            selectedItem = item;
            CategoryViewModel topLevel = null;
            if(item!= null)
            {
                Save_All_DDS.IsEnabled = true;
                var itemParent = item.Parent;

                while (itemParent != null)
                {
                    topLevel = itemParent;
                    itemParent = itemParent.Parent;
                }

                if (item.ItemData != null)
                {
                    if (!topLevel.Name.Equals("UI"))
                    {
                        mViewModel.TextureVM.UpdateTexture(item.ItemData, item.Parent.Name);

                        if (item.Name.Equals(Strings.Face_Paint) || item.Name.Equals(Strings.Equipment_Decals))
                        {
                            tabControl.SelectedIndex = 0;
                            if (mViewModel.ModelVM != null)
                            {
                                mViewModel.ModelVM.ModelTabEnabled = false;
                            }
                        }
                        else
                        {
                            mViewModel.ModelVM.UpdateModel(item.ItemData, item.Parent.Name);
                            mViewModel.ModelVM.ModelTabEnabled = true;
                        }
                    }
                    else
                    {
                        tabControl.SelectedIndex = 0;
                        mViewModel.TextureVM.UpdateTexture(item.ItemData, "UI");
                        mViewModel.ModelVM.ModelTabEnabled = false;
                    }

                }
            }
            else
            {
                Save_All_DDS.IsEnabled = false;
            }
        }

        private void Save_All_DDS_Click(object sender, RoutedEventArgs e)
        {
            mViewModel.TextureVM.SaveAllDDS();
        }

        private void Menu_StartOver_Click(object sender, RoutedEventArgs e)
        {

            string indexBackupFile = Properties.Settings.Default.IndexBackups_Directory + "/{0}.win32.index";
            string index2BackupFile = Properties.Settings.Default.IndexBackups_Directory + "/{0}.win32.index2";

            if (!Helper.IsIndexLocked(true))
            {
                var result = FlexibleMessageBox.Show("Starting over will:\n\n" +
                    "Restore index files to their original state.\n" +
                    "Delete all mods and create new .dat files.\n" +
                    "Delete all .modlist file entries.\n\n" +
                    "Do you want to start over?", "Start Over", MessageBoxButtons.YesNo, MessageBoxIcon.Information);

                if (result == System.Windows.Forms.DialogResult.Yes)
                {

                    RevertAll();

                    var indexFiles = new List<string>();

                    foreach (var indexFile in Info.ModIndexDict)
                    {
                        var indexPath = string.Format(indexBackupFile, indexFile.Key);
                        var index2Path = string.Format(index2BackupFile, indexFile.Key);

                        indexFiles.Add(indexPath);
                        indexFiles.Add(index2Path);
                    }

                    foreach (var i in indexFiles)
                    {
                        if (File.Exists(i))
                        {
                            File.Copy(i, Properties.Settings.Default.FFXIV_Directory + "/" + Path.GetFileName(i), true);
                        }
                    }

                    foreach (var datName in Info.ModDatDict)
                    {
                        var datNum = 0;

                        if (datName.Key.Equals(Strings.ItemsDat))
                        {
                            for (int i = 4; i < 10; i++)
                            {
                                var datPath = string.Format(Info.datDir, datName.Key, i);

                                if (File.Exists(datPath))
                                {
                                    File.Delete(datPath);
                                }
                            }
                        }
                        else if (datName.Key.Equals(Strings.UIDat))
                        {
                            for (int i = 1; i < 5; i++)
                            {
                                var datPath = string.Format(Info.datDir, datName.Key, i);

                                if (File.Exists(datPath))
                                {
                                    File.Delete(datPath);
                                }
                            }
                        }
                    }

                    File.Delete(Properties.Settings.Default.Modlist_Directory);

                    MakeModContainers();
                }

                if (result == System.Windows.Forms.DialogResult.Yes && selectedItem != null)
                {
                    CategoryViewModel topLevel = null;
                    var itemParent = selectedItem.Parent;

                    while (itemParent != null)
                    {
                        topLevel = itemParent;
                        itemParent = itemParent.Parent;
                    }

                    if (selectedItem.ItemData != null)
                    {
                        if (!topLevel.Name.Equals("UI"))
                        {
                            mViewModel.TextureVM.UpdateTexture(selectedItem.ItemData, selectedItem.Parent.Name);

                            if (selectedItem.Name.Equals(Strings.Face_Paint) || selectedItem.Name.Equals(Strings.Equipment_Decals))
                            {
                                tabControl.SelectedIndex = 0;
                                if (mViewModel.ModelVM != null)
                                {
                                    mViewModel.ModelVM.ModelTabEnabled = false;
                                }
                            }
                            else
                            {
                                mViewModel.ModelVM.UpdateModel(selectedItem.ItemData, selectedItem.Parent.Name);
                                mViewModel.ModelVM.ModelTabEnabled = true;
                            }
                        }
                        else
                        {
                            tabControl.SelectedIndex = 0;
                            mViewModel.TextureVM.UpdateTexture(selectedItem.ItemData, "UI");
                            mViewModel.ModelVM.ModelTabEnabled = false;
                        }

                    }
                }
            }
        }

        /// <summary>
        /// Creates files that will contain modded information
        /// </summary>
        private void MakeModContainers()
        {
            foreach (var datName in Info.ModDatDict)
            {
                var datPath = string.Format(Info.datDir, datName.Key, datName.Value);

                if (!File.Exists(datPath))
                {
                    CreateDat.MakeDat();
                    CreateDat.ChangeDatAmounts();
                }
            }

            if (!File.Exists(Properties.Settings.Default.Modlist_Directory))
            {
                CreateDat.CreateModList();
            }
        }

        private void TextBox_TextChanged(object sender, System.Windows.Controls.TextChangedEventArgs e)
        {
            if (searchBox.Text == "")
            {
                // Create an ImageBrush.
                ImageBrush textImageBrush = new ImageBrush();
                textImageBrush.ImageSource =
                    new BitmapImage(
                        new Uri("pack://application:,,,/Resources/search.png")
                    );
                textImageBrush.AlignmentX = AlignmentX.Left;
                // Use the brush to paint the button's background.
                searchBox.Background = textImageBrush;

            }
            else
            {

                searchBox.Background = null;
            }
        }

        private void Menu_Discord_Click(object sender, RoutedEventArgs e)
        {
            System.Diagnostics.Process.Start("https://goo.gl/E1rUxP");
        }

        private void Menu_Tutorials_Click(object sender, RoutedEventArgs e)
        {
            System.Diagnostics.Process.Start("https://textools.dualwield.net/tutorials/");
        }

        private void DXVerButton_Click(object sender, RoutedEventArgs e)
        {
            var dxver = Properties.Settings.Default.DX_Ver;

            if (dxver.Equals(Strings.DX11))
            {
                Properties.Settings.Default.DX_Ver = Strings.DX9;
                Properties.Settings.Default.Save();

                DXVerButton.Content = "DX Version: 9";
            }
            else if (dxver.Equals(Strings.DX9))
            {
                Properties.Settings.Default.DX_Ver = Strings.DX11;
                Properties.Settings.Default.Save();

                DXVerButton.Content = "DX Version: 11";
            }
            else
            {
                DXVerButton.Content = "DX Version: ERROR";
            }

            if ((CategoryViewModel)textureTreeView.SelectedItem != null)
            {
                var itemSelected = (CategoryViewModel)textureTreeView.SelectedItem;
                ((CategoryViewModel)textureTreeView.SelectedItem).IsSelected = false;
                itemSelected.IsSelected = true;
            }
        }

        private void PKEmporium_Click(object sender, RoutedEventArgs e)
        {
            Process.Start("https://prettykittyemporium.blogspot.com/");

        }

        private void NexusMods_Click(object sender, RoutedEventArgs e)
        {
            Process.Start("https://www.nexusmods.com/finalfantasy14");

        }

        private void Menu_MakeModpack_Click(object sender, RoutedEventArgs e)
        {
            MakeModPack mmp = new MakeModPack();
            mmp.Owner = this;
            mmp.Show();
        }

        private void Menu_ImportModpack_Click(object sender, RoutedEventArgs e)
        {
            string mpDir = Properties.Settings.Default.ModPack_Directory;

            if (!Directory.Exists(mpDir))
            {
                Directory.CreateDirectory(mpDir);
            }

            OpenFileDialog ofd = new OpenFileDialog();
            ofd.InitialDirectory = mpDir;
            ofd.Filter = "TexTools ModPack (*.ttmp)|*.ttmp";

            if(ofd.ShowDialog() == System.Windows.Forms.DialogResult.OK)
            {
                ImportModPack imp = new ImportModPack(ofd.FileName);
                imp.Owner = this;
                imp.Show();
            }
        }

        private void Menu_Customize_Click(object sender, RoutedEventArgs e)
        {
            Customize customize = new Customize();
            customize.Owner = this;
            customize.Show();
        }

        private CategoryViewModel FindCategory(string name, IEnumerable<CategoryViewModel> categories)
        {
            foreach (var category in categories)
            {
                if (category.Name == name)
                    return category;

                var childMatch = FindCategory(name, category.Children);
                if (childMatch != null)
                    return childMatch;
            }

            return null;
        }

        private void BatchExport_Click(object sender, RoutedEventArgs e)
        {
            mViewModel.ModelVM.DisableCompositeView();
            var saveDirectory = "E:\\gtfiles\\models";
            var hashes = new ExportHashRepository(Path.Combine(saveDirectory, "repo"));

            var counter = 0;

            // Gear
            var badGear = new HashSet<string>(new[]
            {
                "Doman Iron Hatchet", "Doman Iron Pickaxe",
                "Mammon Lucis", "Kurdalegon Lucis", "Rauni Lucis",
                "Kurdalegon Supra", "Rauni Supra"
            });

            var gear = FindCategory("Gear", mViewModel.Category).Children
                .SelectMany(c => c.Children)
                .Where(i => !badGear.Contains(i.Name));

            foreach (var item in gear)
            {
                if (BatchExportGear(hashes, item, saveDirectory) && counter++ > 15)
                {
                    Debug.WriteLine("Waiting for GC.");
                    GC.Collect();
                    GC.WaitForPendingFinalizers();
                    counter = 0;
                }
            }

            // Minions
            var badMinions = new HashSet<string>(new[]
            {
                "Korpokkur Kid",
                "Magic Bucket"
            });

            var minions = FindCategory("Minions", mViewModel.Category).Children
                .Where(m => !badMinions.Contains(m.Name));

            foreach (var minion in minions)
                BatchExportPrimary(hashes, minion, saveDirectory, "minion");

            // Mounts
            var badMounts = new HashSet<string>(new[]
            {
                "Korpokkur Kolossus"
            });

            var mounts = FindCategory("Mounts", mViewModel.Category).Children
                .Where(m => !badMounts.Contains(m.Name));

            foreach (var mount in mounts)
                BatchExportPrimary(hashes, mount, saveDirectory, "mount");
        }

        private void BatchExportPrimary(ExportHashRepository hashes, CategoryViewModel item, string saveDirectory, string type)
        {
            var categoryPath = Path.Combine(saveDirectory, type);
            Directory.CreateDirectory(categoryPath);

            var itemData = item.ItemData;
            var modelFile = string.Format("{0}-{1}-{2}",
                int.Parse(itemData.PrimaryModelID), int.Parse(itemData.PrimaryModelBody), int.Parse(itemData.PrimaryModelVariant));
            var basePath = Path.Combine(categoryPath, modelFile);
            if (File.Exists(basePath + ".json"))
                return;

            Debug.WriteLine($"Exporting {type} {item.Name}...");

            mViewModel.TextureVM.UpdateTexture(item.ItemData, item.Parent.Name);
            mViewModel.ModelVM.UpdateModel(item.ItemData, item.Parent.Name);
            BatchExportModel(mViewModel.ModelVM, hashes, basePath, type + "/" + modelFile, item);
        }

        private bool BatchExportGear(ExportHashRepository hashes, CategoryViewModel item, string saveDirectory)
        {
            var categoryPath = Path.Combine(saveDirectory, item.ItemData.ItemCategory);
            Directory.CreateDirectory(categoryPath);

            var primaryModelDir = item.ItemData.PrimaryModelKey.ToString().Replace(", ", "-");
            var primaryBasePath = Path.Combine(categoryPath, primaryModelDir);
            var primaryExists = File.Exists(primaryBasePath + ".json");

            string secondaryModelDir = null;
            string secondaryBasePath = null;
            var secondaryExists = true;
            if (item.ItemData.SecondaryModelID != null)
            {
                secondaryModelDir = item.ItemData.SecondaryModelKey.ToString().Replace(", ", "-");
                secondaryBasePath = Path.Combine(categoryPath, secondaryModelDir);
                secondaryExists = File.Exists(secondaryBasePath + ".json");
            }

            if (primaryExists && secondaryExists)
                return false;

            Debug.WriteLine($"Exporting item {item.Name}...");

            mViewModel.TextureVM.UpdateTexture(item.ItemData, item.Parent.Name);
            mViewModel.ModelVM.UpdateModel(item.ItemData, item.Parent.Name);

            if (!primaryExists)
            {
                var primaryModelKey = item.ItemData.ItemCategory + "/" + primaryModelDir;
                BatchExportModel(mViewModel.ModelVM, hashes, primaryBasePath, primaryModelKey, item);
            }

            if (!secondaryExists)
            {
                mViewModel.ModelVM.SelectedPart = mViewModel.ModelVM.PartComboBox[1];
                var secondaryModelKey = item.ItemData.ItemCategory + "/" + secondaryModelDir;
                BatchExportModel(mViewModel.ModelVM, hashes, secondaryBasePath, secondaryModelKey, item);
            }

            return true;
        }

        private void BatchExportModel(ModelViewModel modelViewModel, ExportHashRepository hashes, string path, string modelKey, CategoryViewModel item)
        {
            var metadata = new ExportMetadata();
            metadata.Name = item.Name;

            BatchExportSet(metadata, hashes, modelViewModel);

            var remainingRaces = modelViewModel.RaceComboBox.Where(r => r.ID != modelViewModel.SelectedRace.ID).ToArray();
            foreach (var race in remainingRaces)
            {
                modelViewModel.SelectedRace = race;
                BatchExportSet(metadata, hashes, modelViewModel);
            }

            var metadataJson = JsonConvert.SerializeObject(metadata);
            File.WriteAllText(path + ".json", metadataJson);
        }

        private void BatchExportSet(ExportMetadata metadata, ExportHashRepository hashes, ModelViewModel modelViewModel)
        {
            var set = new ExportSetMetadata();
            set.Name = modelViewModel.SelectedRace.Name;

            for (var i = 0; i < modelViewModel.MeshList.Count; i++)
            {
                var modelTexData = modelViewModel.MeshData[i];
                var modelMeshData = modelViewModel.MeshList[i];

                var modelMetadata = new ExportModelMetadata();
                var objString = string.Join("\r\n", modelMeshData.OBJFileData);
                var objBytes = System.Text.Encoding.ASCII.GetBytes(objString);
                modelMetadata.Obj = hashes.Write(".obj", objBytes);

                modelMetadata.Alpha = hashes.Write(modelTexData.Alpha);
                modelMetadata.Diffuse = hashes.Write(modelTexData.Diffuse);
                modelMetadata.Emissive = hashes.Write(modelTexData.Emissive);
                modelMetadata.Normal = hashes.Write(modelTexData.Normal);
                modelMetadata.Specular = hashes.Write(modelTexData.Specular);
                set.Models.Add(modelMetadata);
            }

            metadata.Sets.Add(set);
        }
    }
}
