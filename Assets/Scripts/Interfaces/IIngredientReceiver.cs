public interface IIngredientReceiver
{
    bool CanReceive(IngredientData data);
    void Receive(IngredientData data);
}
