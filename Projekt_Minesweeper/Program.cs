using System.Numerics;
using System.Text;
using Raylib_cs;

//What every Item equals to:
//0 = Empty
//1-8 = 1-8
//9 = Mine
//10 = Flag
//11 = Unknown

//Getting the monitor size to calculate the maximum of mines that can fit on screen
Raylib.InitWindow(0,0,"Temporary"); //I could only get monitor width/height on a screen with an active window
int MaxWidth = Raylib.GetMonitorWidth(0);// Get monitor width from monitor 0
int MaxHeight = Raylib.GetMonitorHeight(0);
Raylib.CloseWindow();

//Allow user to choose the size of the board and the amount of mines.
Console.Clear();
Console.WriteLine($"Choose a map width. Maximum width is {MaxWidth/25-1}, and the minimum is 10");//Added buffer of one to prevent mouse offset
int MaxX;
int MaxY;
int Minecount;

string WidthString = Console.ReadLine();
while (!int.TryParse(WidthString, out MaxX)||MaxX>MaxWidth/25-1||MaxX<10) //Make sure the amount chosen is withing certain limits and a number
{
    WidthString = Console.ReadLine();
}

Console.WriteLine($"Choose a map height. Maximum width is {MaxHeight/25-1}, and the minimum is 1");  //Make sure the amount chosen is withing certain limits and a number
string HeightString = Console.ReadLine();
while (!int.TryParse(HeightString, out MaxY)||MaxY>MaxHeight/25-1||MaxY<1)
{
    HeightString = Console.ReadLine();
}

Console.WriteLine($"Choose a number of mines. Maximum is {MaxX*MaxY} and the minimum is 1");  //Make sure the amount chosen is withing certain limits and a number
string Minestring = Console.ReadLine();
while (!int.TryParse(Minestring, out Minecount)||Minecount<1)
{
    Minestring=Console.ReadLine();
}

Raylib.InitWindow(25*MaxX,25*MaxY,"MineSweeper");
Raylib.SetTargetFPS(60);

//Defining every variable
bool playing = true;

Texture2D[] Sprites = [Raylib.LoadTexture("img/SquareZero.png"),Raylib.LoadTexture("img/SquareOne.png"), Raylib.LoadTexture("img/SquareTwo.png"),Raylib.LoadTexture("img/SquareThree.png"), Raylib.LoadTexture("img/SquareFour.png"), Raylib.LoadTexture("img/SquareFive.png"), Raylib.LoadTexture("img/SquareSix.png"), Raylib.LoadTexture("img/SquareSeven.png"), Raylib.LoadTexture("img/SquareEight.png"), Raylib.LoadTexture("img/SquareMine.png"), Raylib.LoadTexture("img/SquareUnknownFlag.png"), Raylib.LoadTexture("img/SquareUnknown.png")];
int[,] PlayerKnownMap = new int[MaxX, MaxY];

List<(int, int)> FlagPositions = [];
List<(int,int)> SquaresRevealed = [];
List<(int,int)> SurroundingSquares= [(1,0),(1,1),(0,1),(-1,1),(-1,0),(-1,-1),(0,-1),(1,-1)];
List<(int,int)> MinePositions = [];

Vector2 CursorPos = new(MaxX / 2, MaxY / 2);

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
for (int y = 0;y<MaxY;y++)//Adds the unknown square to all slots in the player known map
{
    for (int x = 0; x<MaxX;x++)
    {
        PlayerKnownMap[x,y]=11;
    }
}

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
    if ((Raylib.IsKeyPressed(KeyboardKey.Enter)||Raylib.IsMouseButtonPressed(MouseButton.Left))&&PlayerKnownMap[(int)CursorPos.X,(int)CursorPos.Y]!=10)    //Looks for mines in the surrounding 8 tiles on enter/left click
    {
        RevealSquare((int)CursorPos.X, (int)CursorPos.Y);
    }
    if (Raylib.IsKeyPressed(KeyboardKey.Space)||Raylib.IsMouseButtonPressed(MouseButton.Right))//Toggle Flags on Space/RightClick
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
    if (Raylib.GetMouseDelta().Length()!=0)//Set CursorPos to the position of the mouse whenever the mouse moves
    {
        CursorPos=new Vector2(Math.Min(MaxX-1, Raylib.GetMouseX()/25), Math.Min(MaxY-1, Raylib.GetMouseY()/25));
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

void RevealSquare(int XPos, int YPos)//Reveals the selected square, rerunning on adjacent squares if they are a 0
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
int MineDetection(List<(int,int)> MinePositions, int XPos, int YPos)//Looks for mines in the surrounding 8 tiles. returns 9 if chosen square is a mine
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