using System;

//Represents a position on the game board (X and Y coordinates)
class Position
{
    public int X { get; set; }
    public int Y { get; set; }

    public Position(int x, int y)
    {
        X = x;
        Y = y;
    }
}
// Represents a cell on game board, a player can occupy a cell
class Cell
{
    public string Occupant { get; set; } = "-"; // Default is empty

    public Cell(string occupant = "-")
    {
        Occupant = occupant;
    }
}
// Represents a player in the game, with name, position and gem count
class Player
{
    public string Name { get; set; }
    public Position Position { get; set; }
    public int GemCount { get; set; } = 0; // Players start with 0 gems

    public Player(string name, Position position)
    {
        Name = name;
        Position = position;
    }
    // Updates player position based on the input direction(player moves only one cell)
    public void Move(char direction)
    {
        switch (direction)
        {
            case 'U': Position.Y = Math.Max(0, Position.Y - 1); break;
            case 'D': Position.Y = Math.Min(5, Position.Y + 1); break;
            case 'L': Position.X = Math.Max(0, Position.X - 1); break;
            case 'R': Position.X = Math.Min(5, Position.X + 1); break;
        }
    }
}
// Represents the game board, containing cells
class Board
{
    public Cell[,] Grid { get; set; } = new Cell[6, 6]; // 6x6 board
    private Random _random = new Random();

    public Board()
    {
        // initialize the board with empty cells
        for (int i = 0; i < 6; i++)
        {
            for (int j = 0; j < 6; j++)
            {
                Grid[i, j] = new Cell();
            }
        }
        PlaceGemsAndObstacles(); // Place gems and obstacles on the board
    }
    // Randomly places gems and obstacles on the board without overlap
    private void PlaceGemsAndObstacles()
    {
       
        Grid[0, 0] = new Cell("P1"); // Player 1 start position
        Grid[5, 5] = new Cell("P2"); // Player 2 start position
        // Place gems and obstacles randomly on the board
        for (int i = 0; i < 10; i++)
        {
            int gemX, gemY, obsX, obsY;
            do { gemX = _random.Next(6); gemY = _random.Next(6); } while (Grid[gemY, gemX].Occupant != "-");
            Grid[gemY, gemX].Occupant = "G";
            do { obsX = _random.Next(6); obsY = _random.Next(6); } while (Grid[obsY, obsX].Occupant != "-");
            Grid[obsY, obsX].Occupant = "O";
        }
    }
    // Displays current state of the board
    public void Display()
    {
        for (int i = 0; i < 6; i++)
        {
            for (int j = 0; j < 6; j++)
            {
                Console.Write(Grid[i, j].Occupant + " ");
            }
            Console.WriteLine();
        }
    }
    // Checks if the players move is valid (boundaries and obstacles)
    public bool IsValidMove(Player player, char direction)
    {
        Position newPos = new Position(player.Position.X, player.Position.Y);
        switch (direction)
        {
            case 'U': newPos.Y--; break;
            case 'D': newPos.Y++; break;
            case 'L': newPos.X--; break;
            case 'R': newPos.X++; break;
        }

        if (newPos.X < 0 || newPos.X > 5 || newPos.Y < 0 || newPos.Y > 5 || Grid[newPos.Y, newPos.X].Occupant == "O")
        {
            return false;
        }

        return true;
    }
    // Updates a players gem count if they moves or removes a gem
    public void CollectGem(Player player)
    {
        if (Grid[player.Position.Y, player.Position.X].Occupant == "G")
        {
            player.GemCount++;
            Grid[player.Position.Y, player.Position.X].Occupant = "-";
        }
    }
}
// Manages overall game logic, Players turns and finding out winners 
class Game
{
    public Board Board { get; set; }
    public Player Player1 { get; set; }
    public Player Player2 { get; set; }
    public Player CurrentTurn { get; set; }
    private int _totalTurns = 30; // 15 turns for each player

    public Game()
    {
        Board = new Board(); // Initialize the game board
        Player1 = new Player("P1", new Position(0, 0)); // Initialize Player 1 
        Player2 = new Player("P2", new Position(5, 5)); // Initialize Player 2
        CurrentTurn = Player1; // Player 1 starts
    }
    // Starts the game loop, Altering turns between players
    public void Start()
    {
        int currentTurnCount = 0;

        while (currentTurnCount < _totalTurns)
        {
            Board.Display();
            Console.WriteLine($"{CurrentTurn.Name}'s turn. Enter direction (U, D, L, R): ");
            char direction = Console.ReadKey(true).KeyChar;
            Console.WriteLine();

            if (Board.IsValidMove(CurrentTurn, direction))
            {
                Board.Grid[CurrentTurn.Position.Y, CurrentTurn.Position.X].Occupant = "-";
                CurrentTurn.Move(direction);
                Board.Grid[CurrentTurn.Position.Y, CurrentTurn.Position.X].Occupant = CurrentTurn.Name;
                Board.CollectGem(CurrentTurn);
                SwitchTurn();
                currentTurnCount++;
            }
            else
            {
                Console.WriteLine("Invalid move. Try again.");
            }
        }

        AnnounceWinner(); // Announce the winner at the end of the game
    }

    // Switches the current turn between Player 1 and 2 
    private void SwitchTurn()
    {
        CurrentTurn = CurrentTurn == Player1 ? Player2 : Player1;
    }

    // Announces the winner based on the number of gems collected 
    private void AnnounceWinner()
    {
        Console.WriteLine("Game Over!");
        if (Player1.GemCount > Player2.GemCount)
        {
            Console.WriteLine($"{Player1.Name} wins with {Player1.GemCount} gems!");
        }
        else if (Player2.GemCount > Player1.GemCount)
        {
            Console.WriteLine($"{Player2.Name} wins with {Player2.GemCount} gems!");
        }
        else
        {
            Console.WriteLine("It's a tie!");
        }
    }
}

class Program
{
    static void Main(string[] args)
    {
        Game game = new Game();
        game.Start(); // Start the game 
    }
}
