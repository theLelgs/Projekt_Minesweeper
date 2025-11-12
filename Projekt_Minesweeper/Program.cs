using System.Numerics;

List<char> Symbols = [' ', '1', '2', '3', '4', '5', '6', '7', '8', 'O'];

Dictionary<string, (int, int, int)> MapSizes = [];
MapSizes["Small"] = (10, 10, 20);
MapSizes["Medium"] = (20, 20, 50);
MapSizes["Large"] = (30, 30,125);


Console.WriteLine("Choose your map size: ");
int linecount = 0;
foreach (string MapSize in MapSizes.Keys)
{
    linecount++;
    (int X, int Y,_) = MapSizes[MapSize];
    Console.WriteLine($"{linecount}. {MapSize}: {X}x{Y}");
}
Console.WriteLine("4. Custom");

ConsoleKeyInfo Input = Console.ReadKey(true);
ConsoleKey[] InputList = [ConsoleKey.D1, ConsoleKey.D2, ConsoleKey.D3, ConsoleKey.D4];
while (!InputList.Contains(Input.Key))
{
    Input = Console.ReadKey(true);
}
if (Input.Key == ConsoleKey.D4)
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
int[,] Map= new int[MaxX,MaxY];

for (int y = 0; y < MaxY; y++)
{
    for (int x = 0; x < MaxX; x++)
    {
        Map[x, y] = 0;
    }
}
for (int a = 0; a < Minecount; a++)
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

Vector2 CursorPos = new(MaxX / 2, MaxY / 2);
while(Input.Key!=ConsoleKey.Escape)
{
    Input = Console.ReadKey(true);
    if (Input.Key==ConsoleKey.W)
    {
        CursorPos -= new Vector2(0, 1);
    }
    if (Input.Key==ConsoleKey.A)
    {
        CursorPos -= new Vector2(1, 0);
    }
    if (Input.Key==ConsoleKey.S)
    {
        CursorPos += new Vector2(0, 1);
    }
    if (Input.Key==ConsoleKey.D)
    {
        CursorPos += new Vector2(1, 0);
    }
    if (Input.Key==ConsoleKey.Enter)
    {
        Console.CursorLeft = 0;
        Console.CursorTop = MaxY + 2;
        Console.Write(MineDetection(Map, (int)CursorPos.X, (int)CursorPos.Y));
    }
    Console.CursorLeft = (int)CursorPos.X;
    Console.CursorTop = (int)CursorPos.Y;
}







int MineDetection(int[,] Map, int XPos, int YPos)
{
    int output = 0;
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
    if (XPos != Map.GetLength(0))
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
    if (YPos!=0)
    {
        if (Map[XPos,YPos-1]==9)
        {
            output++;
        }
    }
    if(YPos!=Map.GetLength(1))
    {
        if (Map[XPos, YPos + 1] == 9)
        {   
            output++;
        }
    }
    return output;
}
void WriteMap(int[,] Map, int MaxX, int MaxY)
{
    for (int y = 0; y < MaxY; y++)
    {
        for (int x = 0; x < MaxX; x++)
        {
            Console.Write(Symbols[Map[x, y]]);
            if (x==MaxX-1)
            {
                Console.Write("\n");
            }
        }
    }
}