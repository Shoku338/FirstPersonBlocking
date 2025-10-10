public interface IInteractable
{
    void OnInteractStart();   // Called once when player starts interacting (e.g., press E)
    void OnInteractHold();    // Called every frame while holding
    void OnInteractEnd();     // Called once when releasing E
    string GetPrompt();
}
