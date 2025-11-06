using SimpleLogManager.Configs;

namespace SimpleLogManager
{
    public interface IBackUpStrategy
    {
        public void BackUp(SLMConfig config);
    }
}
