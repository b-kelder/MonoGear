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
        public static DifficultyLevels Difficulty;
        public static float Volume = 1f;
        public static float MusicVolume = 1f;
        public static float EffectVolume = 1f;

        public SettingsPage()
        {
            this.InitializeComponent();
            AddDifficultyLevels();
            DifficultyComboBox.SelectedIndex = 0;

            MasterVolumeSlider.Value = Volume * 100;
            MusicVolumeSlider.Value = MusicVolume * 100;
            EffectVolumeSlider.Value = EffectVolume * 100;
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
            Difficulty = (DifficultyLevels)Enum.Parse(typeof(DifficultyLevels), DifficultyComboBox.SelectedItem.ToString());
        }

        private void BackButton_Click(object sender, RoutedEventArgs e)
        {
            this.Frame.Navigate(typeof(MenuPage));
        }

        private void MasterVolumeSlider_ValueChanged(object sender, RangeBaseValueChangedEventArgs e)
        {
            Volume = (float)MasterVolumeSlider.Value / 100;
        }

        private void MusicVolumeSlider_ValueChanged(object sender, RangeBaseValueChangedEventArgs e)
        {
            MusicVolume = (float)MusicVolumeSlider.Value / 100;
        }

        private void EffectVolumeSlider_ValueChanged(object sender, RangeBaseValueChangedEventArgs e)
        {
            EffectVolume = (float)EffectVolumeSlider.Value / 100;
        }
    }
}
