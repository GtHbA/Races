using GTA;

namespace Races
{
    class Money
    {
        public static void AddMoney(int sum, int score)
        {
            if (score == 100)
            {
                Game.Player.Money += sum;
                UI.Notify("Выигрыш: " + sum + "\n" +
                    "На счете: " + Game.Player.Money.ToString());

                Vehicle vehicle = Game.Player.Character.CurrentVehicle;
                vehicle.Repair();
            }
            else
            {
                Game.Player.Money -= sum;
                UI.Notify("Проигрыш: -" + sum + "\n" +
                    "На счете: " + Game.Player.Money.ToString());
            }
        }
    }
}