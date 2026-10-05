using System.Globalization;
using System.ComponentModel;
using System.Drawing.Drawing2D;
using System.Windows.Forms;

namespace kalkulator_sederhana;

internal static class Program
{
    [STAThread]
    private static void Main()
    {
        Application.EnableVisualStyles();
        Application.SetCompatibleTextRenderingDefault(false);
        Application.Run(new CalculatorForm());
    }
}

internal sealed class CalculatorForm : Form
{
    internal static readonly Color BackgroundColor = Color.FromArgb(15, 18, 24);
    internal static readonly Color SurfaceColor = Color.FromArgb(23, 28, 37);
    internal static readonly Color NumberColor = Color.FromArgb(34, 40, 51);
    internal static readonly Color UtilityColor = Color.FromArgb(43, 50, 63);
    internal static readonly Color OperatorColor = Color.FromArgb(31, 49, 55);
    internal static readonly Color AccentColor = Color.FromArgb(115, 220, 190);
    internal static readonly Color EqualsColor = Color.FromArgb(235, 183, 109);

    private readonly CalculatorEngine engine = new();
    private readonly Label expressionLabel;
    private readonly Label displayLabel;
    private readonly List<CalculatorButton> operationButtons = [];

    public CalculatorForm()
    {
        Text = "NOVA — Calculator";
        StartPosition = FormStartPosition.CenterScreen;
        MinimumSize = new Size(370, 610);
        Size = new Size(450, 740);
        BackColor = BackgroundColor;
        ForeColor = Color.FromArgb(240, 242, 245);
        Font = new Font("Segoe UI", 10F, FontStyle.Regular, GraphicsUnit.Point);
        AutoScaleMode = AutoScaleMode.Dpi;
        KeyPreview = true;
        DoubleBuffered = true;

        var layout = new TableLayoutPanel
        {
            Dock = DockStyle.Fill,
            BackColor = BackgroundColor,
            ColumnCount = 1,
            RowCount = 4,
            Padding = new Padding(26, 22, 26, 18)
        };
        layout.RowStyles.Add(new RowStyle(SizeType.Absolute, 76));
        layout.RowStyles.Add(new RowStyle(SizeType.Absolute, 152));
        layout.RowStyles.Add(new RowStyle(SizeType.Percent, 100));
        layout.RowStyles.Add(new RowStyle(SizeType.Absolute, 30));
        Controls.Add(layout);

        layout.Controls.Add(CreateHeader(), 0, 0);

        var displayCard = new RoundedPanel
        {
            Dock = DockStyle.Fill,
            BackColor = SurfaceColor,
            CornerRadius = 18,
            Padding = new Padding(22, 17, 22, 16),
            Margin = new Padding(0, 5, 0, 15)
        };
        var displayLayout = new TableLayoutPanel
        {
            Dock = DockStyle.Fill,
            BackColor = Color.Transparent,
            ColumnCount = 1,
            RowCount = 2
        };
        displayLayout.RowStyles.Add(new RowStyle(SizeType.Percent, 38));
        displayLayout.RowStyles.Add(new RowStyle(SizeType.Percent, 62));
        expressionLabel = new Label
        {
            Dock = DockStyle.Fill,
            Text = "",
            ForeColor = Color.FromArgb(137, 149, 163),
            Font = new Font("Segoe UI", 11F),
            TextAlign = ContentAlignment.MiddleRight,
            AutoEllipsis = true
        };
        displayLabel = new Label
        {
            Dock = DockStyle.Fill,
            Text = "0",
            ForeColor = Color.FromArgb(246, 247, 249),
            Font = new Font("Segoe UI Semibold", 34F, FontStyle.Bold),
            TextAlign = ContentAlignment.MiddleRight,
            AutoEllipsis = true
        };
        displayLayout.Controls.Add(expressionLabel, 0, 0);
        displayLayout.Controls.Add(displayLabel, 0, 1);
        displayCard.Controls.Add(displayLayout);
        layout.Controls.Add(displayCard, 0, 1);

        layout.Controls.Add(CreateKeypad(), 0, 2);

        var footer = new Label
        {
            Dock = DockStyle.Fill,
            Text = "KEYBOARD READY     ·     ESC  CLEAR     ·     ENTER  RESULT",
            ForeColor = Color.FromArgb(105, 116, 131),
            Font = new Font("Segoe UI", 8F, FontStyle.Bold),
            TextAlign = ContentAlignment.MiddleCenter
        };
        layout.Controls.Add(footer, 0, 3);
        KeyDown += HandleKeyDown;
    }

    private Control CreateHeader()
    {
        var header = new TableLayoutPanel
        {
            Dock = DockStyle.Fill,
            ColumnCount = 2,
            RowCount = 1,
            BackColor = BackgroundColor,
            Margin = new Padding(0, 0, 0, 8)
        };
        header.ColumnStyles.Add(new ColumnStyle(SizeType.Absolute, 58));
        header.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 100));

        var mark = new RoundedPanel
        {
            Dock = DockStyle.Fill,
            BackColor = Color.FromArgb(31, 55, 53),
            CornerRadius = 14,
            Margin = new Padding(0, 3, 12, 3)
        };
        mark.Controls.Add(new Label
        {
            Dock = DockStyle.Fill,
            Text = "N",
            ForeColor = AccentColor,
            Font = new Font("Segoe UI Semibold", 21F, FontStyle.Bold),
            TextAlign = ContentAlignment.MiddleCenter
        });
        header.Controls.Add(mark, 0, 0);

        var title = new TableLayoutPanel
        {
            Dock = DockStyle.Fill,
            BackColor = BackgroundColor,
            ColumnCount = 1,
            RowCount = 2,
            Margin = new Padding(0)
        };
        title.RowStyles.Add(new RowStyle(SizeType.Percent, 56));
        title.RowStyles.Add(new RowStyle(SizeType.Percent, 44));
        title.Controls.Add(new Label
        {
            Dock = DockStyle.Fill,
            Text = "NOVA",
            ForeColor = Color.FromArgb(241, 243, 246),
            Font = new Font("Segoe UI Semibold", 16F, FontStyle.Bold),
            TextAlign = ContentAlignment.BottomLeft
        }, 0, 0);
        title.Controls.Add(new Label
        {
            Dock = DockStyle.Fill,
            Text = "CALCULATOR  /  STUDIO 01",
            ForeColor = Color.FromArgb(126, 138, 153),
            Font = new Font("Segoe UI", 8F, FontStyle.Bold),
            TextAlign = ContentAlignment.TopLeft
        }, 0, 1);
        header.Controls.Add(title, 1, 0);
        return header;
    }

    private Control CreateKeypad()
    {
        var grid = new TableLayoutPanel
        {
            Dock = DockStyle.Fill,
            BackColor = BackgroundColor,
            ColumnCount = 4,
            RowCount = 5,
            Padding = new Padding(0, 3, 0, 2),
            Margin = new Padding(-6, 0, -6, 0)
        };

        for (var column = 0; column < 4; column++)
        {
            grid.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 25));
        }

        for (var row = 0; row < 5; row++)
        {
            grid.RowStyles.Add(new RowStyle(SizeType.Percent, 20));
        }

        AddButton(grid, "AC", ButtonStyle.Utility, 0, 0, engine.Clear);
        AddButton(grid, "⌫", ButtonStyle.Utility, 1, 0, engine.Backspace);
        AddButton(grid, "±", ButtonStyle.Utility, 2, 0, engine.ToggleSign);
        AddOperationButton(grid, "÷", CalculatorOperation.Divide, 3, 0);

        AddDigitRow(grid, ["7", "8", "9"], 1);
        AddOperationButton(grid, "×", CalculatorOperation.Multiply, 3, 1);
        AddDigitRow(grid, ["4", "5", "6"], 2);
        AddOperationButton(grid, "−", CalculatorOperation.Subtract, 3, 2);
        AddDigitRow(grid, ["1", "2", "3"], 3);
        AddOperationButton(grid, "+", CalculatorOperation.Add, 3, 3);

        AddButton(grid, "%", ButtonStyle.Utility, 0, 4, engine.ApplyPercent);
        AddButton(grid, "0", ButtonStyle.Number, 1, 4, () => engine.InputDigit('0'));
        AddButton(grid, ",", ButtonStyle.Number, 2, 4, engine.InputDecimalSeparator);
        AddButton(grid, "=", ButtonStyle.Equals, 3, 4, () =>
        {
            engine.Calculate();
            SetActiveOperation(null);
        });

        return grid;
    }

    private void AddDigitRow(TableLayoutPanel grid, string[] digits, int row)
    {
        for (var column = 0; column < digits.Length; column++)
        {
            var digit = digits[column][0];
            AddButton(grid, digits[column], ButtonStyle.Number, column, row, () => engine.InputDigit(digit));
        }
    }

    private void AddOperationButton(TableLayoutPanel grid, string text, CalculatorOperation operation, int column, int row)
    {
        var button = AddButton(grid, text, ButtonStyle.Operator, column, row, () =>
        {
            engine.ChooseOperation(operation);
            SetActiveOperation(operation);
        });
        button.Tag = operation;
        operationButtons.Add(button);
    }

    private CalculatorButton AddButton(TableLayoutPanel grid, string text, ButtonStyle style, int column, int row, Action action)
    {
        var button = new CalculatorButton(text, style)
        {
            Dock = DockStyle.Fill,
            Margin = new Padding(6),
            Font = new Font("Segoe UI Semibold", text.Length > 1 ? 15F : 20F, FontStyle.Bold),
            AccessibleName = text == "⌫" ? "Backspace" : text,
            TabStop = false
        };
        button.Click += (_, _) =>
        {
            action();
            RefreshDisplay();
        };
        grid.Controls.Add(button, column, row);
        return button;
    }

    private void SetActiveOperation(CalculatorOperation? operation)
    {
        foreach (var button in operationButtons)
        {
            button.IsActive = operation.HasValue && button.Tag is CalculatorOperation buttonOperation && buttonOperation == operation;
        }
    }

    private void RefreshDisplay()
    {
        expressionLabel.Text = engine.Expression;
        displayLabel.Text = engine.Display;
        displayLabel.Font = new Font("Segoe UI Semibold", engine.Display.Length > 12 ? 22F : engine.Display.Length > 8 ? 28F : 34F, FontStyle.Bold);

        if (engine.Display.StartsWith("Tidak", StringComparison.Ordinal) || engine.Display.StartsWith("Angka", StringComparison.Ordinal) || engine.Display.StartsWith("Hasil", StringComparison.Ordinal))
        {
            displayLabel.ForeColor = Color.FromArgb(245, 151, 139);
            displayLabel.Font = new Font("Segoe UI Semibold", 20F, FontStyle.Bold);
        }
        else
        {
            displayLabel.ForeColor = Color.FromArgb(246, 247, 249);
        }

        if (engine.Display == "0" && engine.Expression.Length == 0)
        {
            SetActiveOperation(null);
        }
    }

    private void HandleKeyDown(object? sender, KeyEventArgs e)
    {
        var handled = true;
        switch (e.KeyCode)
        {
            case Keys.D0:
            case Keys.NumPad0: engine.InputDigit('0'); break;
            case Keys.D1:
            case Keys.NumPad1: engine.InputDigit('1'); break;
            case Keys.D2:
            case Keys.NumPad2: engine.InputDigit('2'); break;
            case Keys.D3:
            case Keys.NumPad3: engine.InputDigit('3'); break;
            case Keys.D4:
            case Keys.NumPad4: engine.InputDigit('4'); break;
            case Keys.D5 when !e.Shift:
            case Keys.NumPad5: engine.InputDigit('5'); break;
            case Keys.D5 when e.Shift: engine.ApplyPercent(); break;
            case Keys.D6:
            case Keys.NumPad6: engine.InputDigit('6'); break;
            case Keys.D7:
            case Keys.NumPad7: engine.InputDigit('7'); break;
            case Keys.D8:
            case Keys.NumPad8: engine.InputDigit('8'); break;
            case Keys.D9:
            case Keys.NumPad9: engine.InputDigit('9'); break;
            case Keys.Add: engine.ChooseOperation(CalculatorOperation.Add); SetActiveOperation(CalculatorOperation.Add); break;
            case Keys.Subtract: engine.ChooseOperation(CalculatorOperation.Subtract); SetActiveOperation(CalculatorOperation.Subtract); break;
            case Keys.Multiply: engine.ChooseOperation(CalculatorOperation.Multiply); SetActiveOperation(CalculatorOperation.Multiply); break;
            case Keys.Divide: engine.ChooseOperation(CalculatorOperation.Divide); SetActiveOperation(CalculatorOperation.Divide); break;
            case Keys.Enter:
            case Keys.OemMinus when e.Shift: engine.Calculate(); SetActiveOperation(null); break;
            case Keys.Oemplus when e.Shift: engine.ChooseOperation(CalculatorOperation.Add); SetActiveOperation(CalculatorOperation.Add); break;
            case Keys.OemMinus: engine.ChooseOperation(CalculatorOperation.Subtract); SetActiveOperation(CalculatorOperation.Subtract); break;
            case Keys.Oem2: engine.ChooseOperation(CalculatorOperation.Divide); SetActiveOperation(CalculatorOperation.Divide); break;
            case Keys.Oem5: engine.ChooseOperation(CalculatorOperation.Divide); SetActiveOperation(CalculatorOperation.Divide); break;
            case Keys.OemPeriod:
            case Keys.Decimal:
            case Keys.Oemcomma: engine.InputDecimalSeparator(); break;
            case Keys.Back: engine.Backspace(); break;
            case Keys.Escape: engine.Clear(); SetActiveOperation(null); break;
            default: handled = false; break;
        }

        if (handled)
        {
            e.Handled = true;
            e.SuppressKeyPress = true;
            RefreshDisplay();
        }
    }
}

internal enum ButtonStyle
{
    Number,
    Utility,
    Operator,
    Equals
}

internal sealed class CalculatorButton : Button
{
    private readonly ButtonStyle style;
    private bool isHovered;

    [Browsable(false)]
    [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
    public bool IsActive { get; set; }

    public CalculatorButton(string text, ButtonStyle style)
    {
        Text = text;
        this.style = style;
        FlatStyle = FlatStyle.Flat;
        FlatAppearance.BorderSize = 0;
        UseVisualStyleBackColor = false;
        SetStyle(ControlStyles.UserPaint | ControlStyles.AllPaintingInWmPaint | ControlStyles.OptimizedDoubleBuffer | ControlStyles.ResizeRedraw, true);
    }

    protected override void OnMouseEnter(EventArgs e)
    {
        isHovered = true;
        Invalidate();
        base.OnMouseEnter(e);
    }

    protected override void OnMouseLeave(EventArgs e)
    {
        isHovered = false;
        Invalidate();
        base.OnMouseLeave(e);
    }

    protected override void OnPaint(PaintEventArgs e)
    {
        e.Graphics.SmoothingMode = SmoothingMode.AntiAlias;
        var bounds = ClientRectangle;
        bounds.Inflate(-1, -1);
        var fill = GetFillColor();
        using var path = RoundedPanel.CreateRoundedPath(bounds, 13);
        using var brush = new SolidBrush(fill);
        e.Graphics.FillPath(brush, path);

        var textColor = style switch
        {
            ButtonStyle.Operator => CalculatorForm.AccentColor,
            ButtonStyle.Equals => Color.FromArgb(30, 35, 40),
            ButtonStyle.Utility => Color.FromArgb(220, 225, 232),
            _ => Color.FromArgb(243, 245, 248)
        };
        TextRenderer.DrawText(e.Graphics, Text, Font, bounds, textColor,
            TextFormatFlags.HorizontalCenter | TextFormatFlags.VerticalCenter | TextFormatFlags.NoPadding | TextFormatFlags.SingleLine);
    }

    private Color GetFillColor()
    {
        if (IsActive)
        {
            return Color.FromArgb(39, 74, 72);
        }

        if (isHovered)
        {
            return style switch
            {
                ButtonStyle.Number => Color.FromArgb(48, 56, 69),
                ButtonStyle.Utility => Color.FromArgb(58, 67, 82),
                ButtonStyle.Operator => Color.FromArgb(42, 66, 70),
                ButtonStyle.Equals => Color.FromArgb(246, 199, 132),
                _ => Color.FromArgb(34, 40, 51)
            };
        }

        return style switch
        {
            ButtonStyle.Number => CalculatorForm.NumberColor,
            ButtonStyle.Utility => CalculatorForm.UtilityColor,
            ButtonStyle.Operator => CalculatorForm.OperatorColor,
            ButtonStyle.Equals => CalculatorForm.EqualsColor,
            _ => CalculatorForm.NumberColor
        };
    }
}

internal sealed class RoundedPanel : Panel
{
    [Browsable(false)]
    [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
    public int CornerRadius { get; set; } = 16;

    public RoundedPanel()
    {
        SetStyle(ControlStyles.UserPaint | ControlStyles.AllPaintingInWmPaint | ControlStyles.OptimizedDoubleBuffer | ControlStyles.ResizeRedraw, true);
    }

    protected override void OnPaint(PaintEventArgs e)
    {
        e.Graphics.SmoothingMode = SmoothingMode.AntiAlias;
        using var path = CreateRoundedPath(ClientRectangle, CornerRadius);
        using var brush = new SolidBrush(BackColor);
        e.Graphics.FillPath(brush, path);
        base.OnPaint(e);
    }

    internal static GraphicsPath CreateRoundedPath(Rectangle bounds, int radius)
    {
        var path = new GraphicsPath();
        if (bounds.Width <= 0 || bounds.Height <= 0)
        {
            return path;
        }

        var diameter = Math.Min(radius * 2, Math.Min(bounds.Width, bounds.Height));
        var arc = new Rectangle(bounds.X, bounds.Y, diameter, diameter);
        path.AddArc(arc, 180, 90);
        arc.X = bounds.Right - diameter;
        path.AddArc(arc, 270, 90);
        arc.Y = bounds.Bottom - diameter;
        path.AddArc(arc, 0, 90);
        arc.X = bounds.X;
        path.AddArc(arc, 90, 90);
        path.CloseFigure();
        return path;
    }
}

internal enum CalculatorOperation
{
    Add,
    Subtract,
    Multiply,
    Divide
}

internal sealed class CalculatorEngine
{
    private readonly CultureInfo culture = CultureInfo.GetCultureInfo("id-ID");
    private double leftOperand;
    private CalculatorOperation? pendingOperation;
    private bool startNewEntry = true;
    private bool hasError;

    public string Display { get; private set; } = "0";
    public string Expression { get; private set; } = "";

    public void InputDigit(char digit)
    {
        if (!char.IsDigit(digit))
        {
            return;
        }

        RecoverFromError();
        Display = startNewEntry || Display == "0"
            ? digit.ToString()
            : Display + digit;
        startNewEntry = false;
        RefreshExpression();
    }

    public void InputDecimalSeparator()
    {
        RecoverFromError();
        if (startNewEntry)
        {
            Display = "0" + culture.NumberFormat.NumberDecimalSeparator;
            startNewEntry = false;
        }
        else if (!Display.Contains(culture.NumberFormat.NumberDecimalSeparator, StringComparison.Ordinal))
        {
            Display += culture.NumberFormat.NumberDecimalSeparator;
        }

        RefreshExpression();
    }

    public void ChooseOperation(CalculatorOperation operation)
    {
        RecoverFromError();
        if (pendingOperation.HasValue && !startNewEntry)
        {
            if (!TryApplyPendingOperation())
            {
                return;
            }
        }

        if (!TryReadDisplay(out leftOperand))
        {
            SetError("Angka tidak valid");
            return;
        }

        pendingOperation = operation;
        startNewEntry = true;
        RefreshExpression();
    }

    public void Calculate()
    {
        if (!pendingOperation.HasValue || startNewEntry)
        {
            return;
        }

        var completedExpression = $"{FormatNumber(leftOperand)} {GetOperationSymbol(pendingOperation.Value)} {Display} =";
        if (!TryApplyPendingOperation())
        {
            return;
        }

        Expression = completedExpression;
        pendingOperation = null;
        startNewEntry = true;
    }

    public void ToggleSign()
    {
        RecoverFromError();
        if (!TryReadDisplay(out var value))
        {
            return;
        }

        Display = FormatNumber(-value);
        startNewEntry = false;
        RefreshExpression();
    }

    public void ApplyPercent()
    {
        RecoverFromError();
        if (!TryReadDisplay(out var value))
        {
            return;
        }

        Display = FormatNumber(value / 100d);
        startNewEntry = false;
        RefreshExpression();
    }

    public void Backspace()
    {
        if (hasError)
        {
            Clear();
            return;
        }

        if (startNewEntry)
        {
            return;
        }

        Display = Display.Length <= 1 || (Display.Length == 2 && Display[0] == '-')
            ? "0"
            : Display[..^1];
        if (Display is "-" or "")
        {
            Display = "0";
        }

        RefreshExpression();
    }

    public void Clear()
    {
        Display = "0";
        Expression = "";
        leftOperand = 0;
        pendingOperation = null;
        startNewEntry = true;
        hasError = false;
    }

    private bool TryApplyPendingOperation()
    {
        if (!pendingOperation.HasValue || !TryReadDisplay(out var rightOperand))
        {
            return true;
        }

        if (pendingOperation == CalculatorOperation.Divide && rightOperand == 0)
        {
            SetError("Tidak dapat membagi 0");
            return false;
        }

        var result = pendingOperation.Value switch
        {
            CalculatorOperation.Add => leftOperand + rightOperand,
            CalculatorOperation.Subtract => leftOperand - rightOperand,
            CalculatorOperation.Multiply => leftOperand * rightOperand,
            CalculatorOperation.Divide => leftOperand / rightOperand,
            _ => throw new InvalidOperationException("Operasi tidak dikenal.")
        };

        if (!double.IsFinite(result))
        {
            SetError("Hasil di luar batas");
            return false;
        }

        leftOperand = result;
        Display = FormatNumber(result);
        startNewEntry = true;
        return true;
    }

    private bool TryReadDisplay(out double value) =>
        double.TryParse(Display, NumberStyles.Float, culture, out value) && double.IsFinite(value);

    private string FormatNumber(double value) => value.ToString("G15", culture);

    private static string GetOperationSymbol(CalculatorOperation operation) => operation switch
    {
        CalculatorOperation.Add => "+",
        CalculatorOperation.Subtract => "−",
        CalculatorOperation.Multiply => "×",
        CalculatorOperation.Divide => "÷",
        _ => "?"
    };

    private void RefreshExpression()
    {
        Expression = pendingOperation.HasValue
            ? $"{FormatNumber(leftOperand)} {GetOperationSymbol(pendingOperation.Value)}{(startNewEntry ? "" : $" {Display}")}"
            : "";
    }

    private void SetError(string message)
    {
        Display = message;
        Expression = "";
        pendingOperation = null;
        startNewEntry = true;
        hasError = true;
    }

    private void RecoverFromError()
    {
        if (hasError)
        {
            Clear();
        }
    }
}
