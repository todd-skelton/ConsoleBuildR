using System.Threading.Tasks;

namespace ConsoleBuildR
{
    public interface IExecutable
    {
        Task Execute(string[] args);
    }
}
