namespace Spikes
{

    /* 
     * components: 
     *  Application Controller
     *      -> Application Shell
     *      -> Screen Manager
     *      -> Screen Layout Manager
     *      -> Menu Management
     *      -> Bootstrapper
     *      -> Logic Mapper? -- no use IStartable on Bootstrapper class, and use configuration to state startup methods on Domain classes.
    */
    public class ApplicationController
    {
        private readonly IApplicationShellPresenter _appShellPresenter;
        private readonly IBootstrapper _bootstrapper;

        public ApplicationController(IApplicationShellPresenter appShellPresenter, IBootstrapper bootstrapper)
        {
            _appShellPresenter = appShellPresenter;
            _bootstrapper = bootstrapper;
        }

        void Start()
        {
            _bootstrapper.InitializeApplication();
            _appShellPresenter.Start();
        }
    }

    public interface IApplicationShellPresenter : IPresenter
    {
        void Initialize();
        IView ApplicationShell { get; set; }

    }

    public interface IPresenter 
    {
        void Start();
    }

    public interface IView
    {
        void Show();
        void Hide();
        void Close();
    }

    public interface IBootstrapper
    {
        void InitializeApplication();
    }
}