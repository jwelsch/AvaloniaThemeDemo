using Avalonia;
using Avalonia.Controls.Metadata;
using Avalonia.Controls.Primitives;
using Avalonia.Interactivity;
using Avalonia.Media;
using Avalonia.VisualTree;
using System.Collections.Generic;
using System.Linq;

namespace AvaloniaThemeDemo.Controls;

public class CheckerboardCell
{
    public int Column { get; }

    public int Row { get; }

    public SolidColorBrush Fill { get; }

    public CheckerboardCell(int column, int row, SolidColorBrush fill)
    {
        Column = column;
        Row = row;
        Fill = fill;
    }

    public CheckerboardCell(int column, int row, Color fill)
        : this(column, row, new SolidColorBrush(fill))
    {
    }
}

[TemplatePart("PART_CheckerboardPresenter", typeof(CheckerboardControl))]
public class CheckerboardControl : TemplatedControl
{
    private UniformGrid? _uniformGrid;

    /// <summary>
    /// Defines the <see cref="Columns"/> property.
    /// </summary>
    public static readonly StyledProperty<int> ColumnsProperty =
        AvaloniaProperty.Register<CheckerboardControl, int>(
            nameof(Columns),
            defaultValue: 8);

    /// <summary>
    /// Gets or sets the number of columns.
    /// </summary>
    public int Columns
    {
        get { return GetValue(ColumnsProperty); }
        set { SetValue(ColumnsProperty, value); }
    }

    /// <summary>
    /// Defines the <see cref="Rows"/> property.
    /// </summary>
    public static readonly StyledProperty<int> RowsProperty =
        AvaloniaProperty.Register<CheckerboardControl, int>(
            nameof(Rows),
            defaultValue: 8);

    /// <summary>
    /// Gets or sets the number of rows.
    /// </summary>
    public int Rows
    {
        get { return GetValue(RowsProperty); }
        set { SetValue(RowsProperty, value); }
    }

    /// <summary>
    /// Defines the <see cref="FirstColor"/> property.
    /// </summary>
    public static readonly StyledProperty<Color> FirstColorProperty =
        AvaloniaProperty.Register<CheckerboardControl, Color>(
            nameof(FirstColor),
            defaultValue: Colors.Black);

    /// <summary>
    /// Gets or sets the color of the first cell and ever other cell after.
    /// </summary>
    public Color FirstColor
    {
        get { return GetValue(FirstColorProperty); }
        set { SetValue(FirstColorProperty, value); }
    }

    /// <summary>
    /// Defines the <see cref="SecondColor"/> property.
    /// </summary>
    public static readonly StyledProperty<Color> SecondColorProperty =
        AvaloniaProperty.Register<CheckerboardControl, Color>(
            nameof(SecondColor),
            defaultValue: Colors.White);

    /// <summary>
    /// Gets or sets the color of the second cell and ever other cell after.
    /// </summary>
    public Color SecondColor
    {
        get { return GetValue(SecondColorProperty); }
        set { SetValue(SecondColorProperty, value); }
    }

    /// <summary>
    /// Defines the <see cref="Cells"/> property.
    /// </summary>
    public static readonly DirectProperty<CheckerboardControl, List<CheckerboardCell>> CellsProperty =
        AvaloniaProperty.RegisterDirect<CheckerboardControl, List<CheckerboardCell>>(
            nameof(Cells),
            o => o.Cells,
            (o, v) => o.Cells = v);

    private List<CheckerboardCell> _cells = new();

    /// <summary>
    /// Gets or sets the cells of the board.
    /// </summary>
    public List<CheckerboardCell> Cells
    {
        get { return _cells; }
        private set { SetAndRaise(CellsProperty, ref _cells, value); }
    }

    static CheckerboardControl()
    {
        AffectsRender<CheckerboardControl>(new AvaloniaProperty[]
        {
            RowsProperty,
            ColumnsProperty,
            FirstColorProperty,
            SecondColorProperty
        });
    }

    public CheckerboardControl()
    {
        Cells = InitializeCells(Rows, Columns, FirstColor, SecondColor);
    }

    private static List<CheckerboardCell> InitializeCells(int rowCount, int columnCount, Color firstColor, Color secondColor)
    {
        var cells = new List<CheckerboardCell>();

        for (var i = 0; i < rowCount * columnCount; i++)
        {
            var column = i % columnCount;
            var row = i / columnCount;
            //var fill = column % 2 == row % 2 ? firstColor : secondColor;
            var fill = column % 2 == 0 ? (row % 2 == 0 ? firstColor : secondColor) : (row % 2 == 0 ? secondColor : firstColor);

            cells.Add(new CheckerboardCell(column, row, fill));
        }

        return cells;
    }

    protected override void OnLoaded(RoutedEventArgs e)
    {
        base.OnLoaded(e);

        _uniformGrid ??= FindVisualChild<UniformGrid>(this.GetVisualChildren());

        if (_uniformGrid != null)
        {
            _uniformGrid.Columns = Columns;
            _uniformGrid.Rows = Rows;
        }
    }

    protected override void OnPropertyChanged(AvaloniaPropertyChangedEventArgs change)
    {
        base.OnPropertyChanged(change);

        if (change.Property.Name == nameof(Rows)
            || change.Property.Name == nameof(Columns)
            || change.Property.Name == nameof(FirstColor)
            || change.Property.Name == nameof(SecondColor))
        {
            if (_uniformGrid != null)
            {
                _uniformGrid.Columns = Columns;
                _uniformGrid.Rows = Rows;
            }

            Cells = InitializeCells(Rows, Columns, FirstColor, SecondColor);
        }
    }

    private T? FindVisualChild<T>(IEnumerable<Visual> children, string? name = null)
    {
        foreach (var child in children)
        {
            if (child is T t
                && (name == null || name == child.Name))
            {
                return t;
            }

            var found = FindVisualChild<T>(child.GetVisualChildren(), name);

            if (found != null)
            {
                return found;
            }
        }

        return default;
    }
}