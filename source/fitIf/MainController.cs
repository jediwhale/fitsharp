using fitSharp.IO;
using fitSharp.Machine.Application;
using fitSharp.Machine.Engine;

namespace fitIf {
    public class MainController {

        public MainController(MainView view) {
            this.view = view;
        }

        public void Initialize() {
            var folderModel = new FileSystemModel();
            new SuiteConfiguration(memory).LoadXml(folderModel.GetPageContent(@"storytest.config.xml"));
            view.ShowTests(new TestFiles(
                    new FileSystemTree(memory.GetItem<Settings>().InputFolder),
                    new FileSystem(memory.GetItem<Settings>().OutputFolder)
                ).Tree);
        }

        readonly MainView view;
        readonly Memory memory = new TypeDictionary();
    }
}
