namespace Tic_Tac_Toe.Client.States
{
    internal class MainMenuState : IState
    {
        public MainMenuState()
        {

        }

        public async Task InvokeAsync()
        {
            while (true)
            {
                Console.WriteLine("Main Menu");
                Console.WriteLine("Please choose action:");
                Console.WriteLine("1 -- Start game");
                Console.WriteLine("2 -- Statistic");
                Console.WriteLine("3 -- Close");
                try
                {
                    var choose = int.Parse(Console.ReadLine());
                    switch (choose)
                    {
                        case 1:
                            break;
                        
                        case 2:
                            break;

                        case 3:
                            return;
                    }
                }
                catch (FormatException ex)
                {
                    Console.WriteLine(ex);
                }

                Console.Clear();
            }
        }

    }
}
