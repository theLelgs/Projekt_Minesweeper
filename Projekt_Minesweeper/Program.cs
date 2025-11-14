using System.Numerics;
using System.Text;
//The symbols used by the map when displaying them. 
List<char> Symbols = [' ', '1', '2', '3', '4', '5', '6', '7', '8', '۞', '?'];


//To allow for more characters to be used in the console
//looked this up tho
Console.OutputEncoding = Encoding.UTF8;


Dictionary<string, (int, int, int)> MapSizes = [];
MapSizes["Small"] = (10, 10, 20);
MapSizes["Medium"] = (20, 15, 50);
MapSizes["Large"] = (25, 25,100);


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
    while (!int.TryParse(tempWidth, out Width))
    {
        Console.Clear();
        tempWidth = Console.ReadLine();
    }
    Console.Clear();
    Console.WriteLine("How tall do you want the board to be?");
    string tempHeight = Console.ReadLine();
    while (!int.TryParse(tempHeight, out Height))
    {
        Console.Clear();
        tempHeight = Console.ReadLine();
    }
    Console.Clear();
    Console.WriteLine("How many mines do you want there to be?");
    string tempMines = Console.ReadLine();
    while (!int.TryParse(tempMines, out Mines) || Mines > Height * Width)
    {
        Console.Clear();
        tempMines = Console.ReadLine();
        Console.Clear();
    }
    MapSizes["Custom"] = (Width, Height, Mines);

}
string[] MapSizeKeyList = ["Small", "Medium", "Large", "Custom"];
(int MaxX, int MaxY, int Minecount) = MapSizes[MapSizeKeyList[int.Parse(Input.KeyChar.ToString()) - 1]];
int[,] Map = new int[MaxX, MaxY];
List<(int, int)> FlagPositions = [];
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
WriteMap(Map, MaxX, MaxY);
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
while(Input.Key!=ConsoleKey.Escape&&playing)
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
    //Looks for mines in the surrounding 8 tiles on enter press
    if (Input.Key == ConsoleKey.Enter)
    {
        RevealSquare(Map, (int)CursorPos.X, (int)CursorPos.Y);
    }
    if (Input.Key==ConsoleKey.Spacebar)
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
    }
    //Moves the cursor to it's correct position
    Console.CursorLeft = (int)CursorPos.X;
    Console.CursorTop = (int)CursorPos.Y;
}


void RevealSquare(int[,] Map, int XPos, int YPos)
{
    Console.Write(Symbols[MineDetection(Map, (int)CursorPos.X, (int)CursorPos.Y)]);
        if (MineDetection(Map, (int)CursorPos.X, (int)CursorPos.Y)==0)
        {
            if (CursorPos.X != 0)
            {
                Console.CursorLeft = (int)CursorPos.X-1;
                Console.CursorTop = (int)CursorPos.Y;
                Console.Write(Symbols[MineDetection(Map, (int)CursorPos.X - 1, (int)CursorPos.Y)]);
                if (CursorPos.Y != 0)
                {
                    Console.CursorLeft = (int)CursorPos.X-1;
                    Console.CursorTop = (int)CursorPos.Y-1;
                    Console.Write(Symbols[MineDetection(Map, (int)CursorPos.X - 1, (int)CursorPos.Y - 1)]);
                }
                if (CursorPos.Y != Map.GetLength(1)-1)
                {
                    Console.CursorLeft = (int)CursorPos.X-1;
                    Console.CursorTop = (int)CursorPos.Y+1;
                    Console.Write(Symbols[MineDetection(Map, (int)CursorPos.X - 1, (int)CursorPos.Y + 1)]);
                }
            }
            if (CursorPos.X != Map.GetLength(0) - 1)
            {
                Console.CursorLeft = (int)CursorPos.X+1;
                Console.CursorTop = (int)CursorPos.Y;
                Console.Write(Symbols[MineDetection(Map, (int)CursorPos.X + 1, (int)CursorPos.Y)]);
                if (CursorPos.Y != 0)
                {
                    Console.CursorLeft = (int)CursorPos.X+1;
                    Console.CursorTop = (int)CursorPos.Y-1;
                    Console.Write(Symbols[MineDetection(Map, (int)CursorPos.X + 1, (int)CursorPos.Y - 1)]);
                }
                if (CursorPos.Y != Map.GetLength(1)-1)
                {
                    Console.CursorLeft = (int)CursorPos.X+1;
                    Console.CursorTop = (int)CursorPos.Y+1;
                    Console.Write(Symbols[MineDetection(Map, (int)CursorPos.X + 1, (int)CursorPos.Y + 1)]);
                }
            }
            if (CursorPos.Y != 0)
            {
                Console.CursorLeft = (int)CursorPos.X;
                Console.CursorTop = (int)CursorPos.Y-1;
                Console.Write(Symbols[MineDetection(Map, (int)CursorPos.X, (int)CursorPos.Y - 1)]);
            }
            if (CursorPos.Y!=Map.GetLength(1)-1)
            {
                Console.CursorLeft = (int)CursorPos.X;
                Console.CursorTop = (int)CursorPos.Y+1;
                Console.Write(Symbols[MineDetection(Map, (int)CursorPos.X, (int)CursorPos.Y+1)]);
            }
        }
}
static int MineDetection(int[,] Map, int XPos, int YPos) //Looks for mines in the 8 surrounding spaces, not checking if the square is out of bounds.
{
    int output = 0;
    if (Map[XPos, YPos] == 9)
    {
        return 9;
    }
    if (XPos != 0)
    {
        if (Map[XPos - 1, YPos] == 9)
        {
            output++;
        }
        if (YPos != 0)
        {
            if (Map[XPos - 1, YPos - 1] == 9)
            {
                output++;
            }
        }
        if (YPos != Map.GetLength(1) - 1)
        {
            if (Map[XPos - 1, YPos + 1] == 9)
            {
                output++;
            }
        }
    }
    if (XPos != Map.GetLength(0) - 1)
    {
        if (Map[XPos + 1, YPos] == 9)
        {
            output++;
        }
        if (YPos != 0)
        {
            if (Map[XPos + 1, YPos - 1] == 9)
            {
                output++;
            }
        }
        if (YPos != Map.GetLength(1) - 1)
        {
            if (Map[XPos + 1, YPos + 1] == 9)
            {
                output++;
            }
        }
    }
    if (YPos != 0)
    {
        if (Map[XPos, YPos - 1] == 9)
        {
            output++;
        }
    }
    if (YPos != Map.GetLength(1) - 1)
    {
        if (Map[XPos, YPos + 1] == 9)
        {
            output++;
        }
    }
    return output;
}

static void WriteMap(int[,] Map, int MaxX, int MaxY) //Writes out the map
{
    for (int y = 0; y < MaxY; y++)
    {
        for (int x = 0; x < MaxX; x++)
        {
            // Console.Write(Symbols[Map[x, y]]);
            Console.Write(" ");
            if (x == MaxX - 1)
            {
                Console.Write("|\n");
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