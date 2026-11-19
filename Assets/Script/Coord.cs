using System;

public readonly struct Coord
{
    public int row { get; }
    public int col { get; }

    public Coord(int row, int col)
    {
        this.row = row;
        this.col = col;
    }
    public static Coord Down => new Coord(1, 0);
    public static Coord Right => new Coord(0, 1);
    public static Coord DiagRight => new Coord(1, 1);
    public static Coord DiagLeft => new Coord(1, -1);
    public static Coord operator +(Coord a, Coord b) => new Coord(a.row + b.row, a.col + b.col);
    public static Coord operator *(int a, Coord b) => new Coord(a * b.row, a * b.col); 

    public override bool Equals(object obj) => obj is Coord other && row == other.row && col == other.col; //! Override Equals và Hash để tối ưu hơn hiệu năng so với default
    public override int GetHashCode() => HashCode.Combine(row, col);
    public static bool operator ==(Coord a, Coord b) => a.row == b.row && a.col == b.col;
    public static bool operator !=(Coord a, Coord b) => !(a == b);
}