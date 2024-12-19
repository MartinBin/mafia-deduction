namespace Mafia_client.Visitor;

public interface IGameComponentVisitor
{
    void Visit(PlayerAvatar avatar);
    void Visit(PlayerLabel label);
    void Visit(PlayerRoleLabel roleLabel);
    void Visit(PlayerPet pet);
    void Visit(PlayerComposite composite);
}