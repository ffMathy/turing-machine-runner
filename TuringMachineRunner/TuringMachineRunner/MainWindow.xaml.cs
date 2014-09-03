using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using TuringMachineRunner.Models;

namespace TuringMachineRunner
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        private Dictionary<string, Dictionary<char, Instruction>> _instructions;
        private char[] _buffer;

        public MainWindow()
        {
            InitializeComponent();

            _instructions = new Dictionary<string, Dictionary<char, Instruction>>();
            _buffer = new char[4096];
        }
    }
}
