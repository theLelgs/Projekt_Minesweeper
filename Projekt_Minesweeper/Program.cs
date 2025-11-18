using System.Numerics;
using System.Text;
using Raylib_cs;
//The symbols used by the map when displaying them. 
List<char> Symbols = [' ', '1', '2', '3', '4', '5', '6', '7', '8', '۞', '?'];
List<(int X,int Y)> SurroundingSquares= [(1,0),(1,1),(0,1),(-1,1),(-1,0),(-1,-1),(0,-1),(1,-1)];

//To allow for more characters to be used in the console
//looked this up tho
Console.OutputEncoding = Encoding.UTF8;
// Raylib.InitWindow(800,600,"MineSweeper");
// Raylib.SetTargetFPS(60);
// Raylib.ToggleBorderlessWindowed();




Dictionary<string, (int MaxX, int MaxY, int Bombs)> MapSizes = [];
MapSizes["Small"] = (10, 10, 15);
MapSizes["Medium"] = (20, 15, 50);
MapSizes["Large"] = (25, 25, 100);


Console.WriteLine("Choose your map size: ");
int linecount = 0;
foreach (string MapSize in MapSizes.Keys) //Writes out the different map options
{
    linecount++;
    (int X, int Y, int mines) = MapSizes[MapSize];
    Console.WriteLine($"{linecount}. {MapSize}: {X}x{Y}. Minecount: {mines}");
}
Console.WriteLine("4. Custom");

ConsoleKeyInfo Input = Console.ReadKey(true);
ConsoleKey[] InputList = [ConsoleKey.D1, ConsoleKey.D2, ConsoleKey.D3, ConsoleKey.D4];
while (!InputList.Contains(Input.Key))
{
    Input = Console.ReadKey(true);
}
if (Input.Key == ConsoleKey.D4)//Allows the user to make a custom board
{
    int Width;
    int Height;
    int Mines;
    Console.Clear();
    Console.WriteLine("How wide do you want the board to be?");
    string tempWidth = Console.ReadLine();
    while (!int.TryParse(tempWidth, out Width)||Width<=0||Width>120)
    {
        Console.Clear();
        Console.WriteLine("How wide do you want the board to be?");
        tempWidth = Console.ReadLine();
    }
    Console.Clear();
    Console.WriteLine("How tall do you want the board to be?");
    string tempHeight = Console.ReadLine();
    while (!int.TryParse(tempHeight, out Height)||Height<=0||Height>30)
    {
        Console.Clear();
        Console.WriteLine("How tall do you want the board to be?");
        tempHeight = Console.ReadLine();
    }
    Console.Clear();
    Console.WriteLine("How many mines do you want there to be?");
    string tempMines = Console.ReadLine();
    while (!int.TryParse(tempMines, out Mines) || Mines > Height * Width)
    {
        Console.Clear();
        Console.WriteLine("How many mines do you want there to be?");
        tempMines = Console.ReadLine();
    }
    Console.Clear();
    MapSizes["Custom"] = (Width, Height, Mines);

}
string[] MapSizeKeyList = ["Small", "Medium", "Large", "Custom"];
(int MaxX, int MaxY, int Minecount) = MapSizes[MapSizeKeyList[int.Parse(Input.KeyChar.ToString()) - 1]];
int[,] Map = new int[MaxX, MaxY];

List<(int, int)> FlagPositions = [];
List<(int,int)> SquaresRevealed = [];



for (int a = 0; a < Minecount; a++) //Adds mines equal to the Minecount integer. No duplicates
    {
        int RandomX = Random.Shared.Next(MaxX);
        int RandomY = Random.Shared.Next(MaxY);
        while (Map[RandomX, RandomY] == 9)
        {
            RandomX = Random.Shared.Next(MaxX);
            RandomY = Random.Shared.Next(MaxY);
        }

        Map[RandomX, RandomY] = 9;

    }

Console.Clear();
WriteMap(MaxX, MaxY);
Console.ReadKey(true);

for (int a = 0;a<MaxX;a++)
{
    for (int b = 0; b < MaxY; b++)
    {
        Console.CursorLeft = a;
        Console.CursorTop = b;
        Console.Write(Symbols[10]);
    }
}

Vector2 CursorPos = new(MaxX / 2, MaxY / 2);
Console.CursorLeft = (int)CursorPos.X;
Console.CursorTop = (int)CursorPos.Y;


bool playing = true;
// while(!Raylib.WindowShouldClose())

// Raylib.BeginDrawing();
// Raylib.ClearBackground(Color.SkyBlue);
while(playing&&Input.Key!=ConsoleKey.Escape)
{
    Input = Console.ReadKey(true);
    //Moves the cursor on WASD input
    if (Input.Key==ConsoleKey.W&&CursorPos.Y>0)
        {
            CursorPos -= new Vector2(0, 1);
        }
    if (Input.Key==ConsoleKey.A&&CursorPos.X>0)
        {
            CursorPos -= new Vector2(1, 0);
        }
    if (Input.Key==ConsoleKey.S&&CursorPos.Y<MaxY-1)
        {
            CursorPos += new Vector2(0, 1);
        }
    if (Input.Key == ConsoleKey.D&&CursorPos.X<MaxX-1)
        {
            CursorPos += new Vector2(1, 0);
        }
    if (Input.Key == ConsoleKey.Enter&&!FlagPositions.Contains(((int,int))(CursorPos.X,CursorPos.Y)))    //Looks for mines in the surrounding 8 tiles on enter press

        {
            RevealSquare(Map, (int)CursorPos.X, (int)CursorPos.Y);
        }
    if (Input.Key==ConsoleKey.Spacebar&&!SquaresRevealed.Contains(((int,int))(CursorPos.X,CursorPos.Y)))
    {
        if (!FlagPositions.Contains(((int, int))(CursorPos.X, CursorPos.Y)))
        {
            Console.Write("T");
            FlagPositions.Add(((int, int))(CursorPos.X, CursorPos.Y));
        }
        else
        {
            Console.Write("?");
            FlagPositions.Remove(((int, int))(CursorPos.X, CursorPos.Y));
        }
        Console.CursorTop=0;
        Console.CursorLeft=MaxX+13;
        Console.Write(Minecount-FlagPositions.Count+"     ");
    }
    Console.CursorLeft = (int)CursorPos.X; //Moves the cursor to it's correct position
    Console.CursorTop = (int)CursorPos.Y;
    
    if (SquaresRevealed.Count+FlagPositions.Count>=MaxX*MaxY&&FlagPositions.Count==Minecount) //Check if player has won
    {
        playing=false;
    }
}
// Raylib.EndDrawing();

Console.Clear();
if (SquaresRevealed.Count+FlagPositions.Count>=MaxX*MaxY)
{
    Console.WriteLine("You Win!");
}
else
{
    Console.WriteLine("You Lose!");
}
Console.ReadKey(true);

void RevealSquare(int[,] Map, int XPos, int YPos)
{
    Console.CursorLeft=XPos;
    Console.CursorTop=YPos;
    Console.Write(Symbols[MineDetection(Map, XPos,YPos)]);
    if (!SquaresRevealed.Contains((XPos, YPos))&&MineDetection(Map,XPos,YPos)!=9)//Dont add to SquaresRevealed if it already contains it
    {
    SquaresRevealed.Add((XPos,YPos));
    }
    if (MineDetection(Map, XPos, YPos)==0)//Reveal all surrounding squares, rerunning RevealSquare on neighbouring 0
    {
        foreach ((int,int) PositionMod in SurroundingSquares)
        {
            if (!(XPos+PositionMod.Item1>MaxX-1||XPos+PositionMod.Item1<0||YPos+PositionMod.Item2>MaxY-1||YPos+PositionMod.Item2<0)&&!SquaresRevealed.Contains((XPos+PositionMod.Item1,YPos+PositionMod.Item2)))
            {
                Console.CursorLeft=XPos+PositionMod.Item1;
                Console.CursorTop=YPos+PositionMod.Item2;
                SquaresRevealed.Add((XPos+PositionMod.Item1,YPos+PositionMod.Item2));
                Console.Write(Symbols[MineDetection(Map, XPos+PositionMod.Item1, YPos+PositionMod.Item2)]);
                if (MineDetection(Map, XPos+PositionMod.Item1, YPos+PositionMod.Item2)==0)
                {
                    RevealSquare(Map,XPos+PositionMod.Item1,YPos+PositionMod.Item2);
                }
            }
        }
    }
    if (MineDetection(Map, XPos, YPos)==9)//Detect if player revealed a mine
    {
        playing=false;
        Console.ReadKey(true);
    }
}
int MineDetection(int[,] Map, int XPos, int YPos)
{
    int output=0;
    if (Map[XPos, YPos] == 9)
    {
        return 9;
    }
    foreach ((int, int) PositionMod in SurroundingSquares)
    {
        if (!(XPos+PositionMod.Item1>MaxX-1||XPos+PositionMod.Item1<0||YPos+PositionMod.Item2>MaxY-1||YPos+PositionMod.Item2<0))
        {
            if (Map[XPos+PositionMod.Item1, YPos+PositionMod.Item2]==9)
            {
                output++;
            }
        }
    }
    return output;
}
void WriteMap(int MaxX, int MaxY) //Writes out the map
{
    for (int y = 0; y < MaxY; y++)
    {
        for (int x = 0; x < MaxX; x++)
        {
            Console.Write("?");
            if (x == MaxX - 1)
            {
                Console.Write("|");
                if (y==0)
                {
                    Console.Write($"Bombs left: {Minecount}");
                }
                Console.Write("\n");
            }
        }
        if (y == MaxY-1)
        {
            for (int x = 0; x < MaxX; x++)
            {
                Console.Write("=");
            }
        }
    }
}