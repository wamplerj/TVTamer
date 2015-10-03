using System.Linq;
using Moq;
using NUnit.Framework;
using TvTamer.Core;
using TvTamer.Core.Configuration;
using TvTamer.Core.FileSystem;
using TvTamer.Core.Models;

namespace TvTamer.UnitTests
{
    [TestFixture]
    public class EpisodeProcessorTests
    {
        private EpisodeProcessorSettings _settings = new EpisodeProcessorSettings()
        {
            DownloadFolder = "DownloadFolder",
            TvLibraryFolder = "TvLibFolder",
            FileExtentions = ".mp4,.avi",
            DryRun = false
        };

        [Test]
        public void NoEpisodesFoundInDownloadFolder()
        {

            var source = new Mock<IDirectory>();
            var destination = new Mock<IDirectory>();

            var fileSystem = new Mock<IFileSystem>();
            fileSystem.Setup(fsf => fsf.GetDirectory("DownloadFolder")).Returns(source.Object);
            fileSystem.Setup(fsf => fsf.GetDirectory("TvLibFolder")).Returns(destination.Object);

            var analyticService = new Mock<IAnalyticsService>();

            var processor = new EpisodeProcessor(_settings, null, fileSystem.Object, analyticService.Object);
            processor.ProcessDownloadedEpisodes();

            source.Verify(s => s.EnumerateFiles(It.IsAny<string>(), true), Times.AtLeastOnce);
               
        }

        [Test]
        public void MatchingEpisodesCopiedToDestinationFolder()
        {
            var files = new[]
            {
                "DownloadFolder\\The.Walking.Dead.S05E12.720p.HDTV.x264-KILLERS.mp4"
            };

            var source = new Mock<IDirectory>();
            source.Setup(s => s.EnumerateFiles("*", true)).Returns(files);
            source.SetupGet(s => s.Path).Returns("DownloadFolder");
            var destination = new Mock<IDirectory>();
            destination.SetupGet(s => s.Path).Returns("TvLibFolder");

            var seriesDestinationFolder = new Mock<IDirectory>();
            seriesDestinationFolder.Setup(sdf => sdf.EnumerateFiles(It.IsAny<string>(), It.IsAny<bool>())).Returns(Enumerable.Empty<string>());
            seriesDestinationFolder.Setup(sdf => sdf.Exists()).Returns(true);

            var episodeFile = new Mock<IFile>();
            episodeFile.Setup(ef => ef.Extension).Returns(".mp4");
            episodeFile.Setup(ef => ef.DirectoryName).Returns("DownloadFolder");

            var fileSystemFactory = GetFileSystemFactory(source, destination, seriesDestinationFolder, "DownloadFolder\\The.Walking.Dead.S05E12.720p.HDTV.x264-KILLERS.mp4", episodeFile);

            var episode = new TvEpisode() {Season = 5, EpisodeNumber = 12, Title = "Some Title"};

            var tvService = new Mock<ITvService>();
            tvService.Setup(ts => ts.GetEpisodeBySeriesName(It.IsAny<string>(), It.IsAny<int>(), It.IsAny<int>(), true))
                .Returns(episode);

            var analyticService = new Mock<IAnalyticsService>();

            var processor = new EpisodeProcessor(_settings, tvService.Object, fileSystemFactory.Object, analyticService.Object);
            processor.ProcessDownloadedEpisodes();

            source.Verify(s => s.EnumerateFiles(It.IsAny<string>(), true), Times.AtLeastOnce);
            seriesDestinationFolder.Verify(sdf => sdf.Exists(), Times.Once);

            //Validate Source File was copied
            episodeFile.Verify(ef => ef.Copy(It.IsAny<string>()), Times.Once);

            //Validate Episode was updated in database
            tvService.Verify(c => c.SaveChanges(), Times.Once);

            episodeFile.Verify(ef => ef.Delete(), Times.Once);
        }


        [Test]
        public void FolderIsDeletedAfterFileIsProcessed()
        {

            var file = "DownloadFolder\\The.Walking.Dead.S05E12.720p.HDTV.x264-KILLERS\\blah.mp4";

            var source = new Mock<IDirectory>();
            source.SetupGet(s => s.Path).Returns("DownloadFolder");
            var destination = new Mock<IDirectory>();
            var seriesDestinationFolder = new Mock<IDirectory>();
            var sourceEpisodeFolder = new Mock<IDirectory>();
            sourceEpisodeFolder.Setup(sef => sef.Exists()).Returns(true);

            var episodeFile = new Mock<IFile>();
            episodeFile.Setup(ef => ef.Extension).Returns(".mp4");
            episodeFile.Setup(ef => ef.DirectoryName).Returns("DownloadFolder\\The.Walking.Dead.S05E12.720p.HDTV.x264-KILLERS\\");

            var fileSystemFactory = GetFileSystemFactory(source, destination, seriesDestinationFolder, file, episodeFile);
            fileSystemFactory.Setup(fsf => fsf.GetDirectory("DownloadFolder\\The.Walking.Dead.S05E12.720p.HDTV.x264-KILLERS")).Returns(sourceEpisodeFolder.Object);

            var tvService = new Mock<ITvService>();
            var analyticService = new Mock<IAnalyticsService>();

            var processor = new EpisodeProcessor(_settings, tvService.Object, fileSystemFactory.Object, analyticService.Object);
            processor.DeleteSourceFile(file);

            sourceEpisodeFolder.Verify(sef => sef.Delete(true), Times.Once);
        }

        private Mock<IFileSystem> GetFileSystemFactory(Mock<IDirectory> source, Mock<IDirectory> destination, Mock<IDirectory> seriesDestinationFolder, string episodeFilePath, Mock<IFile> episodeFile)
        {
            var fileSystemFactory = new Mock<IFileSystem>();
            fileSystemFactory.Setup(fsf => fsf.GetDirectory("DownloadFolder")).Returns(source.Object);
            fileSystemFactory.Setup(fsf => fsf.GetDirectory("TvLibFolder")).Returns(destination.Object);
            fileSystemFactory.Setup(fsf => fsf.GetDirectory("TvLibFolder\\The Walking Dead\\Season 05\\"))
                .Returns(seriesDestinationFolder.Object);
            fileSystemFactory.Setup(
                fsf => fsf.GetFile(episodeFilePath))
                .Returns(episodeFile.Object);
            return fileSystemFactory;
        }

    }
}
