using System.Numerics;
using System.Text;
using Raylib_cs;

//What every Item equals to:
//0 = Empty
//1-8 = 1-8
//9 = Mine
//10 = Flag
//11 = Unknown


//To allow for more characters to be used in the console
//looked this up tho
// Console.OutputEncoding = Encoding.UTF8;
Raylib.InitWindow(25,25,"Temporary");
int monitor = Raylib.GetCurrentMonitor();
int MaxWidth = Raylib.GetMonitorWidth(monitor);
int MaxHeight = Raylib.GetMonitorHeight(monitor);
Raylib.CloseWindow();
Console.Clear();
Console.WriteLine($"Choose a map width. Maximum width is {MaxWidth/25}, and the minimum is 10");
int width;
int height;
int Minecount;
string widthString = Console.ReadLine();
while (!int.TryParse(widthString, out width)||width>MaxWidth/25||width<10)
{
    widthString = Console.ReadLine();
}
Console.WriteLine($"Choose a map height. Maximum width is {MaxHeight/25}, and the minimum is 1");
string heightString = Console.ReadLine();
while (!int.TryParse(heightString, out height)||height>MaxHeight/25||height<1)
{
    heightString = Console.ReadLine();
}
Console.WriteLine($"Choose a number of mines. Maximum is {width*height} and the minimum is 1");
string Minestring = Console.ReadLine();
while (!int.TryParse(Minestring, out Minecount)||Minecount<1)
{
    Minestring=Console.ReadLine();
}


Raylib.InitWindow(25*width,25*height,"MineSweeper");
Raylib.SetTargetFPS(60);
int MaxX = width;
int MaxY=height;


int[,] Map = new int[MaxX, MaxY];
int[,] PlayerKnownMap = new int[MaxX, MaxY];
for (int y = 0;y<MaxY;y++)
{
    for (int x = 0; x<MaxX;x++)
    {
        PlayerKnownMap[x,y]=11;
    }
}

List<(int, int)> FlagPositions = [];
List<(int,int)> SquaresRevealed = [];
List<(int,int)> SurroundingSquares= [(1,0),(1,1),(0,1),(-1,1),(-1,0),(-1,-1),(0,-1),(1,-1)];
List<(int,int)> MinePositions = [];
Texture2D[] Sprites = [Raylib.LoadTexture("img/SquareZero.png"),Raylib.LoadTexture("img/SquareOne.png"), Raylib.LoadTexture("img/SquareTwo.png"),Raylib.LoadTexture("img/SquareThree.png"), Raylib.LoadTexture("imt/SquareFour.png"), Raylib.LoadTexture("img/SquareFive.png"), Raylib.LoadTexture("img/SquareSix.png"), Raylib.LoadTexture("img/SquareSeven.png"), Raylib.LoadTexture("img/SquareEight.png"), Raylib.LoadTexture("img/SquareMine.png"), Raylib.LoadTexture("img/SquareUnknownFlag.png"), Raylib.LoadTexture("img/SquareUnknown.png")];


for (int a = 0; a < Minecount; a++) //Adds mines equal to the Minecount integer. No duplicates
    {
        int RandomX = Random.Shared.Next(MaxX);
        int RandomY = Random.Shared.Next(MaxY);
        while (MinePositions.Contains((RandomX, RandomY)))
        {
            RandomX = Random.Shared.Next(MaxX);
            RandomY = Random.Shared.Next(MaxY);
        }

        MinePositions.Add((RandomX, RandomY));

    }
Vector2 CursorPos = new(MaxX / 2, MaxY / 2);
bool playing = true;
while(!Raylib.WindowShouldClose()&&playing)
{
    Raylib.BeginDrawing();
    Raylib.ClearBackground(Color.SkyBlue);
    List<Color> colors = [Color.DarkBrown, Color.Blue, Color.Gray, Color.Gold, Color.Magenta, Color.Lime, Color.Brown, Color.Brown, Color.Brown, Color.Red, Color.Brown, Color.Brown, Color.Brown, Color.Brown, Color.Brown, Color.Brown];
    for (int x = 0;x<MaxX;x++)
    {
        for (int y = 0;y<MaxY;y++)
        {
            Raylib.DrawTexture(Sprites[PlayerKnownMap[x,y]],x*25,y*25, Color.White);
        }
    }
    Raylib.DrawRectangleLines((int)CursorPos.X*25, (int)CursorPos.Y*25, 25, 25, Color.Red);
    //Moves the cursor on WASD input
    if (Raylib.IsKeyPressed(KeyboardKey.W)&&CursorPos.Y>0)
            {
                CursorPos -= new Vector2(0, 1);
            }
    if (Raylib.IsKeyPressed(KeyboardKey.A)&&CursorPos.X>0)
            {
                CursorPos -= new Vector2(1, 0);
            }
    if (Raylib.IsKeyPressed(KeyboardKey.S)&&CursorPos.Y<MaxY-1)
            {
                CursorPos += new Vector2(0, 1);
            }
    if (Raylib.IsKeyPressed(KeyboardKey.D)&&CursorPos.X<MaxX-1)
            {
                CursorPos += new Vector2(1, 0);
            }
    if ((Raylib.IsKeyPressed(KeyboardKey.Enter)||Raylib.IsMouseButtonPressed(MouseButton.Left))&&PlayerKnownMap[(int)CursorPos.X,(int)CursorPos.Y]!=10)    //Looks for mines in the surrounding 8 tiles on enter press
    {
        RevealSquare((int)CursorPos.X, (int)CursorPos.Y);
    }
    if (Raylib.IsKeyPressed(KeyboardKey.Space)||Raylib.IsMouseButtonPressed(MouseButton.Right))
    {
        if (PlayerKnownMap[(int)CursorPos.X,(int)CursorPos.Y]==11)
        {
            PlayerKnownMap[(int)CursorPos.X,(int)CursorPos.Y]=10;
            FlagPositions.Add(((int,int))(CursorPos.X, CursorPos.Y));
        }
        else if (PlayerKnownMap[(int)CursorPos.X,(int)CursorPos.Y]==10)
        {
            PlayerKnownMap[(int)CursorPos.X,(int)CursorPos.Y]=11;
            FlagPositions.Remove(((int,int))(CursorPos.X, CursorPos.Y));
        }
    }
    if (Raylib.GetMouseDelta().Length()!=0)
    {
        CursorPos=Raylib.GetMousePosition()/25;
    }
    if (SquaresRevealed.Count+FlagPositions.Count>=MaxX*MaxY&&FlagPositions.Count==Minecount) //Check if player has won
    {
        playing=false;
    }
    Raylib.EndDrawing(); 
}
if (SquaresRevealed.Count+FlagPositions.Count>=MaxX*MaxY)
{
    //WIN
    
}
else
{

    //LOSE
    
}

void RevealSquare(int XPos, int YPos)//Reveals the selected square, rerunning on adjacent squares if it's a 0
{
    if (!SquaresRevealed.Contains((XPos, YPos))&&MineDetection(MinePositions, XPos,YPos)!=9)//Dont add to SquaresRevealed if it already contains it
    {
    SquaresRevealed.Add((XPos,YPos));
    }
    PlayerKnownMap[XPos,YPos]=MineDetection(MinePositions, XPos,YPos);
    if (MineDetection(MinePositions, XPos, YPos)==0)//Reveal all surrounding squares, rerunning RevealSquare on neighbouring 0
    {
        foreach ((int,int) PositionMod in SurroundingSquares)
        {
            if (!(XPos+PositionMod.Item1>MaxX-1||XPos+PositionMod.Item1<0||YPos+PositionMod.Item2>MaxY-1||YPos+PositionMod.Item2<0)&&!SquaresRevealed.Contains((XPos+PositionMod.Item1,YPos+PositionMod.Item2)))
            {
                SquaresRevealed.Add((XPos+PositionMod.Item1,YPos+PositionMod.Item2));
                PlayerKnownMap[XPos+PositionMod.Item1,YPos+PositionMod.Item2]=MineDetection(MinePositions, XPos+PositionMod.Item1,YPos+PositionMod.Item2);
                if (MineDetection(MinePositions, XPos+PositionMod.Item1, YPos+PositionMod.Item2)==0)
                {
                    RevealSquare(XPos+PositionMod.Item1,YPos+PositionMod.Item2);
                }
            }
        }
    }
    if (MineDetection(MinePositions, XPos, YPos)==9)//Detect if player revealed a mine
    {
        playing=false;
    }
}
int MineDetection(List<(int,int)> MinePositions, int XPos, int YPos)//Looks for mines in the surrounding 8 tiles
{
    int output=0;
    if (MinePositions.Contains((XPos,YPos)))
    {
        return 9;
    }
    foreach ((int, int) PositionMod in SurroundingSquares)
    {
        if (!(XPos+PositionMod.Item1>MaxX-1||XPos+PositionMod.Item1<0||YPos+PositionMod.Item2>MaxY-1||YPos+PositionMod.Item2<0))
        {
            if (MinePositions.Contains((XPos+PositionMod.Item1, YPos+PositionMod.Item2)))
            {
                output++;
            }
        }
    }
    return output;
}