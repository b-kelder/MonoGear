using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Navigation;

using MonoGear.Engine.Audio;

// The Blank Page item template is documented at http://go.microsoft.com/fwlink/?LinkId=234238

namespace MonoGear
{

    public enum DifficultyLevels
    {
        Intern,
        Professional,
        Veteran,
        JamesBond
    }

    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class SettingsPage : Page
    {
        private static DifficultyLevels selectedDifficulty;

        public SettingsPage()
        {
            this.InitializeComponent();
            AddDifficultyLevels();
            DifficultyComboBox.SelectedIndex = 0;
        }

       
        private void AddDifficultyLevels()
        {
            foreach (var item in Enum.GetValues(typeof(DifficultyLevels)))
            {
                DifficultyComboBox.Items.Add(item);
            }    
        }

        private void DifficultyComboBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            selectedDifficulty = (DifficultyLevels)Enum.Parse(typeof(DifficultyLevels), DifficultyComboBox.SelectedItem.ToString());
        }

        private void BackButton_Click(object sender, RoutedEventArgs e)
        {
            this.Frame.Navigate(typeof(MenuPage));
        }

        private void MasterVolumeSlider_ValueChanged(object sender, RangeBaseValueChangedEventArgs e)
        {
        }

        private void MusicVolumeSlider_ValueChanged(object sender, RangeBaseValueChangedEventArgs e)
        {
        }

        private void EffectVolumeSlider_ValueChanged(object sender, RangeBaseValueChangedEventArgs e)
        {
        }

        public static DifficultyLevels GetDifficulty()
        {
            return selectedDifficulty;
        }
    }
}
