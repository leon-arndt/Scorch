public interface VA_Subject  {
    //Observerpattern Interface
    void Register(VA_Observer va);
    void Unregister(VA_Observer va);
    void NotifyObserver();
}
