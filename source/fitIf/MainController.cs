using System.IO;
using fitSharp.Fit.Application;
using fitSharp.Fit.Engine;
using fitSharp.Fit.Model;
using fitSharp.Fit.Operators;
using fitSharp.Fit.Service;
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
            var inputPath = memory.GetItem<Settings>().InputFolder;
            var outputPath = memory.GetItem<Settings>().OutputFolder;
            var processor = new CellProcessorBase(memory, memory.GetItem<CellOperators>());
            view.ShowTests(new TestFiles(
                    new FileFolder(inputPath),
                    new FileFolder(outputPath),
                    new StoryTestSuite(memory.GetItem<FileExclusions>(), s => IsExecutable(processor, s))
                ).Tree);
        }

        public void SelectTest(TestFile file) {
            view.ShowResult(System.IO.Path.GetFullPath(System.IO.Path.Combine(memory.GetItem<Settings>().OutputFolder, file.FullName + ".html")));
        }

        static bool IsExecutable(CellProcessor processor, string content) {
            var parsedInput = processor.Compose(new StoryTestString(content));
            return parsedInput != null && parsedInput.Branches.Count > 0;
        }

        readonly MainView view;
        readonly Memory memory = new TypeDictionary();
    }
}
