namespace PRG1_Calculator
{
    public partial class Form1 : Form
    {
        private bool calculationCompleted = false; // denna visade sig viktig för att avgöra status på variablerna
        private double accumulator = 0;
        private string userInput = "";
        private string operation = "";
        private double operand = 0;
        private float originalFontSizeTextBox, originalFontSizeButton, originalFormWidth, originalFormHeight;

        public Form1()
        {
            InitializeComponent();
            // ClientSize.Height och ClientSize.Width ger storleken på formuläret utan titel och fönsterram
            originalFormHeight = this.ClientSize.Height;
            originalFormWidth = this.ClientSize.Width;
            this.Resize += new EventHandler(Form_Resize);
            InitializeFontSizes();
        }

        #region hanterar fontstorlekar
        // Responsiva fonter finns inte i Windows Forms, som hos MAUI, WPF eller andra senare gränssnitt.
        // Man kan ändå simulera detta, om än ganska omständigt.
        private void Form_Resize(object sender, EventArgs e)
        {
            // this.Height och this.Width ger storleken på formuläret inklusive titel och fönsterram
            float scaleFactor = Math.Max(this.Width / (float)originalFormWidth, this.Height / (float)originalFormHeight);
            AdjustControlFonts(this.Controls, scaleFactor);
        }

        private void InitializeFontSizes()
        {
            originalFontSizeTextBox = txtB_Show.Font.Size;
            originalFontSizeButton = FindButtonFontSize(this.Controls);
        }

        /// <summary>
        /// När knappar och textbox (två Controllers) placeras i en tableLayoutPanel, blir de en del av 
        /// TableLayoutPanel:s ControlCollection, därav "omvägen" till Control.ControlCollection.
        /// Fontens storlek görs alltså beroende av den dymansiak storleken för vår tableLayoutPanel,
        /// som i sin tur är beroende av formulärets storlek.
        /// </summary>
        /// <param name="controls">The controls.</param>
        /// <returns></returns>
        private float FindButtonFontSize(Control.ControlCollection controls)
        {
            foreach (Control ctrl in controls)
            {
                if (ctrl is Button btn)
                {
                    return btn.Font.Size; // Returnerar fontstorleken på den första hittade knappen
                }
                else if (ctrl.HasChildren)
                {
                    float size = FindButtonFontSize(ctrl.Controls);
                    if (size != 0) return size;
                }
            }
            return 0; // Om ingen knapp hittades
        }

        private void AdjustControlFonts(Control.ControlCollection controls, float scaleFactor)
        {
            foreach (Control control in controls)
            {
                if (control is Button btn)
                {
                    btn.Font = new Font(btn.Font.FontFamily, originalFontSizeButton * scaleFactor, btn.Font.Style);
                }
                else if (control is TextBox txtB)
                {
                    txtB.Font = new Font(txtB.Font.FontFamily, originalFontSizeTextBox * scaleFactor, txtB.Font.Style);
                }
                // Rekursivt anrop för barnkontroller om några
                if (control.HasChildren)
                {
                    AdjustControlFonts(control.Controls, scaleFactor);
                }
            }
        }

        #endregion

        // Det kan vara smart att ha denna som en egen metod, för att hålla reda 
        // på vad som ska finnas i de olika variablerna
        private void UpdateInput(string value)
        {
            if (calculationCompleted)
            {
                btn_clear.PerformClick();
                txtB_Show.Text = "";
                calculationCompleted = false;
            }

            if (txtB_Show.Text == "0" && value != "." && !"C".Contains(value))
            {
                txtB_Show.Text = "";
            }

            if (value == "+" || value == "-" || value == "*" || value == "/")
            {
                if (!string.IsNullOrEmpty(operation) && !string.IsNullOrEmpty(userInput))
                {
                    btn_equal.PerformClick();
                }
                else if (!string.IsNullOrEmpty(userInput))
                {
                    operand = double.Parse(userInput);
                    accumulator = operand;
                    userInput = "";
                }
                operation = value;
                txtB_Show.Text += "" + value + "";
            }

            else if (value == "C")
            {
                btn_clear.PerformClick();
            }
            else
            {
                userInput += value;
                txtB_Show.Text += value;
            }
        }

        private void NumberButton_Click(object sender, EventArgs e)
        {
            Button button = (Button)sender;
            UpdateInput(button.Text);
        }

        private void OperatorButton_Click(object sender, EventArgs e)
        {
            Button button = (Button)sender;
            string selectedOperation = button.Text;

            if (!string.IsNullOrEmpty(userInput))
            {
                if (!double.TryParse(userInput, out operand))
                {
                    txtB_Show.Text = "Ogiltig inmatning";
                    return;
                }

                if (!string.IsNullOrEmpty(operation))
                {
                    Calculate();
                }
                else
                {
                    accumulator = operand;
                }
                operation = selectedOperation;
                userInput = "";
            }
            else if (accumulator != 0)
            {
                operation = selectedOperation;
            }
            else
            {
                txtB_Show.Text = "Ange ett nummer först";
            }
            txtB_Show.Text = accumulator + "" + operation + "";

            calculationCompleted = false;
        }

        private void btn_equal_Click(object sender, EventArgs e)
        {
            if (!string.IsNullOrEmpty(userInput))
            {
                if (!double.TryParse(userInput, out operand))
                {
                    txtB_Show.Text = "Ogiltig inmatning";
                    return;
                }
            }
            else if (accumulator != 0 && !string.IsNullOrEmpty(operation))
            {
                operand = accumulator;
            }
            else
            {
                return;
            }

            Calculate();

            txtB_Show.Text = accumulator.ToString();
            userInput = "";
            operation = "";

            calculationCompleted = true;
        }

        // Nu har denna del blivit självständig, för att kunna anropas
        // från mer än ett ställe
        private void Calculate()
        {
            switch (operation)
            {
                case "+":
                    accumulator += operand;
                    break;
                case "-":
                    accumulator -= operand;
                    break;
                case "/":
                    if (operand == 0)
                    {
                        txtB_Show.Text = "Fel: Division med noll";
                        return;
                    }
                    accumulator /= operand;
                    break;
                case "*":
                    accumulator *= operand;
                    break;
            }
        }

        private void btn_clear_Click(object sender, EventArgs e)
        {
            userInput = "";
            accumulator = 0;
            operation = "";
            txtB_Show.Text = "0";
        }

        private void btn_storeInMemory_Click(object sender, EventArgs e)
        {
            txtB_Show.Text = "Kommande funktion";
        }

        private void btn_catchFromMemory_Click(object sender, EventArgs e)
        {
            txtB_Show.Text = "Kommande funktion";
        }
    }
}
