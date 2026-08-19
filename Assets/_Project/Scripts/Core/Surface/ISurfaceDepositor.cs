using MaterialAccumulation.Core.Deposition;

namespace MaterialAccumulation.Core.Surface
{
    /// <summary>Приём свипа владельцем состояния. Единственный путь записи в поле высот.</summary>
    public interface ISurfaceDepositor
    {
        public void Deposit(in DepositionStroke stroke);
    }
}
