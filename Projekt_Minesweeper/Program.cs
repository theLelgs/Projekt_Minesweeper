using System.Numerics;
using System.Text;
using Raylib_cs;

//What every Item equals to:
//0 = Empty
//1-8 = 1-8
//9 = Mine
//10 = Flag
//11 = Unknown Tile

//TODO:
//Add Win/Lose screen ✔️
//Allow for player to loop ✔️




//Getting the monitor size to calculate the maximum of mines that can fit on screen
Raylib.InitWindow(0,0,"Temporary"); //I could only get monitor width/height on a screen with an active window
int MonitorWidth = Raylib.GetMonitorWidth(0);// Get monitor width from monitor 0
int MonitorHeight = Raylib.GetMonitorHeight(0);
Raylib.CloseWindow();

//Allow user to choose the size of the board and the amount of mines.
Console.Clear();
Console.WriteLine($"Choose a map width. Maximum width is {MonitorWidth/25-1}, and the minimum is 10");//Added buffer of one to prevent mouse offset
int MaxX;
int MaxY;
int Minecount;

string WidthString = Console.ReadLine();
while (!int.TryParse(WidthString, out MaxX)||MaxX>MonitorWidth/25-1||MaxX<10) //Make sure the amount chosen is withing certain limits and a number
{
    WidthString = Console.ReadLine();
}

Console.WriteLine($"Choose a map height. Maximum height is {MonitorHeight/25-3}, and the minimum is 1");  //Make sure the amount chosen is withing certain limits and a number
string HeightString = Console.ReadLine();
while (!int.TryParse(HeightString, out MaxY)||MaxY>MonitorHeight/25-3||MaxY<1)
{
    HeightString = Console.ReadLine();
}

Console.WriteLine($"Choose a number of mines. Maximum is {MaxX*MaxY} and the minimum is 1");  //Make sure the amount chosen is withing certain limits and a number
string Minestring = Console.ReadLine();
while (!int.TryParse(Minestring, out Minecount)||Minecount<1)
{
    Minestring=Console.ReadLine();
}

Raylib.InitWindow(25*MaxX,25*MaxY+50,"MineSweeper");
Raylib.SetTargetFPS(60);

//Defining every variable
bool playing = true;
string gamestate = "Starting";

Texture2D[] Sprites = [Raylib.LoadTexture("img/SquareZero.png"),Raylib.LoadTexture("img/SquareOne.png"), Raylib.LoadTexture("img/SquareTwo.png"),Raylib.LoadTexture("img/SquareThree.png"), Raylib.LoadTexture("img/SquareFour.png"), Raylib.LoadTexture("img/SquareFive.png"), Raylib.LoadTexture("img/SquareSix.png"), Raylib.LoadTexture("img/SquareSeven.png"), Raylib.LoadTexture("img/SquareEight.png"), Raylib.LoadTexture("img/SquareMine.png"), Raylib.LoadTexture("img/SquareUnknownFlag.png"), Raylib.LoadTexture("img/SquareUnknown.png")];
int[,] PlayerKnownMap = new int[MaxX, MaxY];

List<(int, int)> FlagPositions = [];
List<(int,int)> SquaresRevealed = [];
List<(int,int)> SurroundingSquares= [(1,0),(1,1),(0,1),(-1,1),(-1,0),(-1,-1),(0,-1),(1,-1)];
List<(int,int)> MinePositions = [];

Vector2 CursorPos=Vector2.Create(0);

while (playing&&!Raylib.WindowShouldClose())
{
    if (gamestate == "Starting")
    {
        FlagPositions.Clear();
        MinePositions.Clear();
        SquaresRevealed.Clear();
        CursorPos = Vector2.Create(MaxX/2, MaxY/2);
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
        gamestate="InGame";
    }

    if (gamestate=="InGame")
    {
        Raylib.BeginDrawing();
        Raylib.ClearBackground(Color.SkyBlue);
        Raylib.DrawText(CursorPos.X+":"+CursorPos.Y,0,0,10,Color.Black);
        for (int x = 0;x<MaxX;x++)
        {
            for (int y = 0;y<MaxY;y++)
            {
                Raylib.DrawTexture(Sprites[PlayerKnownMap[x,y]],x*25,(y+2)*25, Color.White);
            }
        }
        Raylib.DrawRectangleLines((int)CursorPos.X*25, (int)(CursorPos.Y+2)*25, 25, 25, Color.Red);
        Raylib.DrawRectangle(0,0,MaxX*25,50, Color.LightGray);
        Raylib.DrawText($"Mines left: {Minecount-FlagPositions.Count}",0,0,30,Color.Black);
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
            CursorPos=new Vector2(Math.Min(MaxX-1, Raylib.GetMouseX()/25), Math.Max(0, Math.Min(MaxY-1+2, Raylib.GetMouseY()/25)-2));
        }
        if (SquaresRevealed.Count+FlagPositions.Count>=MaxX*MaxY&&FlagPositions.Count==Minecount) //Check if player has won
        {
            gamestate="Win";
        }
        Raylib.EndDrawing(); 
    }
    if (gamestate=="Win"||gamestate=="Lose")
    {
        int FontSize=Math.Min(MaxY*25/4, MaxX*31/16);
        Raylib.BeginDrawing();
        Raylib.ClearBackground(Color.SkyBlue);
        Raylib.DrawRectangle(0,0,MaxX*25, (MaxY+2)*25, Color.LightGray);
        Raylib.DrawText($"You {gamestate}!", 0, 0, FontSize, Color.Black);
        Raylib.DrawText("Do you want to play again?", 0, FontSize, FontSize, Color.Black);
        Raylib.DrawText("1. Yes", 0, FontSize*2, FontSize, Color.Black);
        Raylib.DrawText("2. No", 0, FontSize*3, FontSize, Color.Black);

        if (Raylib.IsKeyPressed(KeyboardKey.One))
        {
            gamestate="Starting";
        }
        if (Raylib.IsKeyPressed(KeyboardKey.Two))
        {
            playing=false;
            gamestate=null;
        }
        Raylib.EndDrawing();
    }
}

void RevealSquare(int XPos, int YPos)//Reveals the selected square, rerunning on adjacent squares if they are a 0
{
    if (!SquaresRevealed.Contains((XPos, YPos))&&MineDetection(MinePositions, XPos,YPos)!=9)//Dont add to SquaresRevealed if it already contains it
    {
    SquaresRevealed.Add((XPos,YPos));
    }
    if (PlayerKnownMap[XPos,YPos]!=11)
    {
        int SurroundingFlagCount=0;
        foreach ((int,int) PositionOffset in SurroundingSquares)
        {
            if (FlagPositions.Contains((XPos+PositionOffset.Item1, YPos+PositionOffset.Item2)))
            {
                SurroundingFlagCount++;
            }
        }
        if (SurroundingFlagCount==PlayerKnownMap[XPos,YPos])
        {
            foreach ((int,int) PositionOffset in SurroundingSquares)
            {
                if (!(XPos+PositionOffset.Item1>MaxX-1||XPos+PositionOffset.Item1<0||YPos+PositionOffset.Item2>MaxY-1||YPos+PositionOffset.Item2<0)&&!SquaresRevealed.Contains((XPos+PositionOffset.Item1,YPos+PositionOffset.Item2))&&!FlagPositions.Contains((XPos+PositionOffset.Item1, YPos+PositionOffset.Item2)))
                {
                    SquaresRevealed.Add((XPos+PositionOffset.Item1,YPos+PositionOffset.Item2));
                    PlayerKnownMap[XPos+PositionOffset.Item1,YPos+PositionOffset.Item2]=MineDetection(MinePositions, XPos+PositionOffset.Item1,YPos+PositionOffset.Item2);
                    if (MineDetection(MinePositions, XPos+PositionOffset.Item1, YPos+PositionOffset.Item2)==0)
                    {
                        RevealSquare(XPos+PositionOffset.Item1,YPos+PositionOffset.Item2);
                    }
                    if (MineDetection(MinePositions, XPos+PositionOffset.Item1, YPos+PositionOffset.Item2)==9)
                    {
                        gamestate="Lose";
                    }
                }
            }
        }
    }
    PlayerKnownMap[XPos,YPos]=MineDetection(MinePositions, XPos,YPos);
    if (MineDetection(MinePositions, XPos, YPos)==0)//Reveal all surrounding squares, rerunning RevealSquare on neighbouring 0
    {
        foreach ((int,int) PositionOffset in SurroundingSquares)
        {
            if (!(XPos+PositionOffset.Item1>MaxX-1||XPos+PositionOffset.Item1<0||YPos+PositionOffset.Item2>MaxY-1||YPos+PositionOffset.Item2<0)&&!SquaresRevealed.Contains((XPos+PositionOffset.Item1,YPos+PositionOffset.Item2)))
            {
                if (PlayerKnownMap[XPos+PositionOffset.Item1, YPos+PositionOffset.Item2]!=10)
                {
                    
                SquaresRevealed.Add((XPos+PositionOffset.Item1,YPos+PositionOffset.Item2));
                PlayerKnownMap[XPos+PositionOffset.Item1,YPos+PositionOffset.Item2]=MineDetection(MinePositions, XPos+PositionOffset.Item1,YPos+PositionOffset.Item2);
                if (MineDetection(MinePositions, XPos+PositionOffset.Item1, YPos+PositionOffset.Item2)==0)
                {
                    RevealSquare(XPos+PositionOffset.Item1,YPos+PositionOffset.Item2);
                }
                }
            }
        }
    }
    if (MineDetection(MinePositions, XPos, YPos)==9)//Detect if player revealed a mine
    {
        gamestate="Lose";
    }
    
}
int MineDetection(List<(int,int)> MinePositions, int XPos, int YPos)//Looks for mines in the surrounding 8 tiles. returns 9 if chosen square is a mine
{
    int output=0;
    if (MinePositions.Contains((XPos,YPos)))
    {
        return 9;
    }
    foreach ((int, int) PositionOffset in SurroundingSquares)
    {
        if (!(XPos+PositionOffset.Item1>MaxX-1||XPos+PositionOffset.Item1<0||YPos+PositionOffset.Item2>MaxY-1||YPos+PositionOffset.Item2<0))
        {
            if (MinePositions.Contains((XPos+PositionOffset.Item1, YPos+PositionOffset.Item2)))
            {
                output++;
            }
        }
    }
    return output;
}