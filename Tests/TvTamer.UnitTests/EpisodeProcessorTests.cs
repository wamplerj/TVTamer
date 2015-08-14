using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Linq.Expressions;
using Moq;
using NUnit.Framework;
using TvTamer.Core.Configuration;
using TvTamer.Core.FileSystem;
using TvTamer.Core.Models;
using TvTamer.Core.Persistance;

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

            var fileSystemFactory = new Mock<IFileSystemFactory>();
            fileSystemFactory.Setup(fsf => fsf.GetDirectory("DownloadFolder")).Returns(source.Object);
            fileSystemFactory.Setup(fsf => fsf.GetDirectory("TvLibFolder")).Returns(destination.Object);

            var processor = new EpisodeProcessor(_settings, null, fileSystemFactory.Object);
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
            var series = new TvSeries() {Name = "The Walking Dead"};
            series.Episodes.Add(episode);

            var episodeQueryable = new List<TvEpisode> { episode }.AsQueryable();
            var seriesQueryable = new List<TvSeries> { series }.AsQueryable();
            
            var seriesSet = new Mock<MockableDbSetWithExtensions<TvSeries>>();
            seriesSet.As<IQueryable<TvSeries>>().Setup(m => m.Provider).Returns(seriesQueryable.Provider);
            seriesSet.As<IQueryable<TvSeries>>().Setup(m => m.Expression).Returns(seriesQueryable.Expression);
            seriesSet.As<IQueryable<TvSeries>>().Setup(m => m.ElementType).Returns(seriesQueryable.ElementType);
            seriesSet.As<IQueryable<TvSeries>>().Setup(m => m.GetEnumerator()).Returns(seriesQueryable.GetEnumerator());
            seriesSet.Setup(ss => ss.Include(It.IsAny<string>())).Returns(seriesSet.Object);

            var episodeSet = new Mock<MockableDbSetWithExtensions<TvEpisode>>();
            episodeSet.As<IQueryable<TvEpisode>>().Setup(m => m.Provider).Returns(episodeQueryable.Provider);
            episodeSet.As<IQueryable<TvEpisode>>().Setup(m => m.Expression).Returns(episodeQueryable.Expression);
            episodeSet.As<IQueryable<TvEpisode>>().Setup(m => m.ElementType).Returns(episodeQueryable.ElementType);
            episodeSet.As<IQueryable<TvEpisode>>().Setup(m => m.GetEnumerator()).Returns(episodeQueryable.GetEnumerator());

            var context = new Mock<ITvContext>();
            context.Setup(c => c.TvSeries).Returns(seriesSet.Object);
            context.Setup(c => c.TvEpisodes).Returns(episodeSet.Object);

            var processor = new EpisodeProcessor(_settings, context.Object, fileSystemFactory.Object);
            processor.ProcessDownloadedEpisodes();

            source.Verify(s => s.EnumerateFiles(It.IsAny<string>(), true), Times.AtLeastOnce);
            seriesDestinationFolder.Verify(sdf => sdf.Exists(), Times.Once);

            //Validate Source File was copied
            episodeFile.Verify(ef => ef.Copy(It.IsAny<string>()), Times.Once);
            
            //Validate Episode was updated in database
            context.Verify(c => c.SaveChanges(), Times.Once);

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

            var context = new Mock<ITvContext>();

            var processor = new EpisodeProcessor(_settings, context.Object, fileSystemFactory.Object);
            processor.DeleteSourceFile(file);

            sourceEpisodeFolder.Verify(sef => sef.Delete(true), Times.Once);



        }
        private Mock<IFileSystemFactory> GetFileSystemFactory(Mock<IDirectory> source, Mock<IDirectory> destination, Mock<IDirectory> seriesDestinationFolder, string episodeFilePath, Mock<IFile> episodeFile)
        {
            var fileSystemFactory = new Mock<IFileSystemFactory>();
            fileSystemFactory.Setup(fsf => fsf.GetDirectory("DownloadFolder")).Returns(source.Object);
            fileSystemFactory.Setup(fsf => fsf.GetDirectory("TvLibFolder")).Returns(destination.Object);
            fileSystemFactory.Setup(fsf => fsf.GetDirectory("TvLibFolder\\The Walking Dead\\Season 05\\"))
                .Returns(seriesDestinationFolder.Object);
            fileSystemFactory.Setup(
                fsf => fsf.GetFile(episodeFilePath))
                .Returns(episodeFile.Object);
            return fileSystemFactory;
        }

        public abstract class MockableDbSetWithExtensions<T> : DbSet<T> where T : class
        {
            public abstract void AddOrUpdate(params T[] entities);
            public abstract void AddOrUpdate(Expression<Func<T, object>> identifierExpression, params T[] entities);
        }

    }
}
