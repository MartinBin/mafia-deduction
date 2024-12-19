using System.Windows.Controls;
using Mafia_client.Factory;
using Mafia_client.Visitor;

public class PlayerPet : GameElement
{
    public PlayerPet(string petType)
    {
        IPet pet = PetFactory.CreatePet(petType);
        if (pet != null)
        {
            element = pet.GetPetImage();
        }
    }
    public override void Render(Canvas canvas, double x, double y)
    {
        if (element != null)
        {
            Canvas.SetLeft(element, x + 30);  // Place to the right of avatar
            Canvas.SetTop(element, y - 15);   // Align vertically with avatar
            canvas.Children.Add(element);
        }
    }
    
    public override void Accept(IGameComponentVisitor visitor)
    {
        visitor.Visit(this);
    }
}