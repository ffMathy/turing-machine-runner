using System;
using System.Collections.Generic;
using System.Diagnostics;
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
        private const int BufferSize = 1024 * 128;

        private Dictionary<string, Dictionary<char, Instruction>> _states;
        private string _currentState;
        private char[] _buffer;

        public MainWindow()
        {
            InitializeComponent();

            if (Debugger.IsAttached)
            {
                Input.Text = "_XXX_XXXX";
            }

            _states = new Dictionary<string, Dictionary<char, Instruction>>();
            _buffer = new char[BufferSize];

            RunButton.Click += RunButton_Click;
        }

        private async void RunButton_Click(object sender, RoutedEventArgs e)
        {
            RunButton.IsEnabled = false;

            var program = InputProgram.Text;
            var input = Input.Text;

            LoadProgram(program);
            await RunInput(input);

            RunButton.IsEnabled = true;
        }

        private char NormalizeSymbol(char input)
        {
            if (input == '_') return '\0';
            return input;
        }

        private async Task RunInput(string input)
        {
            input.CopyTo(0, _buffer, 0, Math.Min(input.Length, _buffer.Length));

            OutputGrid.Visibility = Visibility.Visible;

            var i = 0;
            var bufferEnd = 0;
            while (i < _buffer.Length)
            {

                if (i > _buffer.Length)
                {
                    MessageBox.Show("Buffer overflow. The tape can't be longer than " + _buffer.Length + " bytes.", "Woops", MessageBoxButton.OK, MessageBoxImage.Exclamation);
                    break;
                }

                if (i < 0)
                {
                    break;
                }

                var symbol = NormalizeSymbol(_buffer[i]);

                var state = _states[_currentState];
                if (!state.ContainsKey(symbol))
                {
                    break;
                }

                var instruction = state[symbol];
                _buffer[i] = instruction.WriteSymbol;
                _currentState = instruction.GoToState;

                switch (instruction.Direction) {

                    case Direction.Left:
                        i--;
                        break;

                    case Direction.Right:
                        i++;
                        break;

                    case Direction.Halt:
                    default:
                        break;

                }

                if (i > bufferEnd)
                {
                    bufferEnd = i;
                }
                else if(_buffer[bufferEnd] == '\0' && bufferEnd > 0)
                {
                    bufferEnd -= 1;
                }

                Result.Text = new string(_buffer, 0, bufferEnd);

                if (instruction.WriteSymbol != symbol)
                {
                    await Task.Delay((int)Delay.Value);
                }
            }
        }

        private void LoadProgram(string program)
        {
            _states.Clear();
            _currentState = null;
            _buffer = new char[BufferSize];

            Result.Text = string.Empty;

            var programLines = program.Split(new[] { '\n', '\r' }, StringSplitOptions.RemoveEmptyEntries);
            foreach (var line in programLines.Where(l => !l.StartsWith("//")))
            {
                var split = line.Split(new[] { ' ', ',' }, StringSplitOptions.RemoveEmptyEntries);

                if (split.Length == 5 && split[1].Length == 1 && split[3].Length == 1)
                {
                    var currentState = split[0];
                    var inputSymbol = NormalizeSymbol(split[1][0]);
                    var goToState = split[2];
                    var outputSymbol = NormalizeSymbol(split[3][0]);
                    var direction = Direction.Halt;

                    switch (split[4].ToUpper())
                    {

                        case "<":
                        case "L":
                        case "LEFT":
                            direction = Direction.Left;
                            break;

                        case ">":
                        case "R":
                        case "RIGHT":
                            direction = Direction.Right;
                            break;

                    }

                    if (!_states.ContainsKey(currentState))
                    {
                        if (_currentState == null)
                        {
                            _currentState = currentState;
                        }
                        _states.Add(currentState, new Dictionary<char, Instruction>());
                    }

                    var state = _states[currentState];
                    if (!state.ContainsKey(inputSymbol))
                    {
                        state.Add(inputSymbol, null);
                    }

                    state[inputSymbol] = new Instruction()
                    {
                        Direction = direction,
                        WriteSymbol = outputSymbol,
                        GoToState = goToState
                    };
                }
            }
        }
    }
}
