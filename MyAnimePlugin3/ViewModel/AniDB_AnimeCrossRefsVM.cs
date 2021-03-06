﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace MyAnimePlugin3.ViewModel
{
	public class AniDB_AnimeCrossRefsVM
	{
		public int AnimeID { get; set; }

		public AniDB_AnimeCrossRefsVM()
		{
			CrossRef_AniDB_TvDB = null;
			TvDBSeries = null;
			TvDBEpisodes = new List<TvDB_EpisodeVM>();
			TvDBImageFanarts = new List<TvDB_ImageFanartVM>();
			TvDBImagePosters = new List<TvDB_ImagePosterVM>();
			TvDBImageWideBanners = new List<TvDB_ImageWideBannerVM>();

			CrossRef_AniDB_MovieDB = null;
			MovieDB_Movie = null;
			MovieDBPosters = new List<MovieDB_PosterVM>();
			MovieDBFanarts = new List<MovieDB_FanartVM>();

            CrossRef_AniDB_Trakt = new List<CrossRef_AniDB_TraktVM>();
			TraktShow = new List<Trakt_ShowVM>();
			TraktImageFanart = new List<Trakt_ImageFanartVM>();
			TraktImagePoster = new List<Trakt_ImagePosterVM>();

			AllPosters = new List<PosterContainer>();
			AllFanarts = new List<FanartContainer>();
		}

		private bool tvDBCrossRefExists = false;
		public bool TvDBCrossRefExists
		{
			get { return tvDBCrossRefExists; }
			set { tvDBCrossRefExists = value; }
		}


		public List<TvDB_SeriesVM> TvDBSeries { get; set; }
		public List<CrossRef_AniDB_TvDBVMV2> CrossRef_AniDB_TvDB  { get; set; }
		public List<TvDB_EpisodeVM> TvDBEpisodes  { get; set; }
		public List<TvDB_ImageFanartVM> TvDBImageFanarts  { get; set; }
		public List<TvDB_ImagePosterVM> TvDBImagePosters  { get; set; }
		public List<TvDB_ImageWideBannerVM> TvDBImageWideBanners  { get; set; }

		private bool movieDBCrossRefExists = false;
		public bool MovieDBCrossRefExists
		{
			get { return movieDBCrossRefExists; }
			set { movieDBCrossRefExists = value; }
		}

		public MovieDB_MovieVM MovieDB_Movie  { get; set; }
		public CrossRef_AniDB_OtherVM CrossRef_AniDB_MovieDB  { get; set; }
		public List<MovieDB_FanartVM> MovieDBFanarts  { get; set; }
		public List<MovieDB_PosterVM> MovieDBPosters  { get; set; }

		public List<PosterContainer> AllPosters  { get; set; }
		public List<FanartContainer> AllFanarts  { get; set; }

		private bool traktCrossRefExists = false;
		public bool TraktCrossRefExists
		{
			get { return traktCrossRefExists; }
			set { traktCrossRefExists = value; }
		}

		public List<CrossRef_AniDB_TraktVM> CrossRef_AniDB_Trakt  { get; set; }
		public List<Trakt_ShowVM> TraktShow  { get; set; }
		public List<Trakt_ImageFanartVM> TraktImageFanart  { get; set; }
		public List<Trakt_ImagePosterVM> TraktImagePoster  { get; set; }

		public void Populate(JMMServerBinary.Contract_AniDB_AnimeCrossRefs details)
		{
			AnimeID = details.AnimeID;

			AniDB_AnimeVM anime = JMMServerHelper.GetAnime(AnimeID);
			if (anime == null) return;
			
			

			// Trakt
            if (details.CrossRef_AniDB_Trakt != null)
            {
                CrossRef_AniDB_Trakt = new List<CrossRef_AniDB_TraktVM>();
                foreach (JMMServerBinary.Contract_CrossRef_AniDB_TraktV2 contract in details.CrossRef_AniDB_Trakt)
                {
                    CrossRef_AniDB_TraktVM xref = new CrossRef_AniDB_TraktVM(contract);
                    CrossRef_AniDB_Trakt.Add(xref);
                }
            }

            if (details.TraktShows != null)
            {
                TraktShow = new List<Trakt_ShowVM>();
                foreach (JMMServerBinary.Contract_Trakt_Show contract in details.TraktShows)
                {
                    Trakt_ShowVM show = new Trakt_ShowVM(contract);
                    TraktShow.Add(show);
                }
            }

            foreach (JMMServerBinary.Contract_Trakt_ImageFanart contract in details.TraktImageFanarts)
            {
                bool isDefault = false;
                if (anime != null && anime.DefaultFanart != null && anime.DefaultFanart.ImageParentType == (int)ImageEntityType.Trakt_Fanart
                    && anime.DefaultFanart.ImageParentID == contract.Trakt_ImageFanartID)
                {
                    isDefault = true;
                }

                Trakt_ImageFanartVM traktFanart = new Trakt_ImageFanartVM(contract);
                traktFanart.IsImageDefault = isDefault;
                TraktImageFanart.Add(traktFanart);

                AllFanarts.Add(new FanartContainer(ImageEntityType.Trakt_Fanart, traktFanart));
            }

            foreach (JMMServerBinary.Contract_Trakt_ImagePoster contract in details.TraktImagePosters)
            {
                bool isDefault = false;
                if (anime != null && anime.DefaultPoster != null && anime.DefaultPoster.ImageParentType == (int)ImageEntityType.Trakt_Poster
                    && anime.DefaultPoster.ImageParentID == contract.Trakt_ImagePosterID)
                {
                    isDefault = true;
                }

                Trakt_ImagePosterVM traktPoster = new Trakt_ImagePosterVM(contract);
                traktPoster.IsImageDefault = isDefault;
                TraktImagePoster.Add(traktPoster);

                AllPosters.Add(new PosterContainer(ImageEntityType.Trakt_Poster, traktPoster));
            }

            if ((CrossRef_AniDB_Trakt == null || CrossRef_AniDB_Trakt.Count == 0) ||
                (TraktShow == null || TraktShow.Count == 0))
				TraktCrossRefExists = false;
			else
				TraktCrossRefExists = true;

			// TvDB
			if (details.CrossRef_AniDB_TvDB != null)
			{
				CrossRef_AniDB_TvDB = new List<CrossRef_AniDB_TvDBVMV2>();
				foreach (JMMServerBinary.Contract_CrossRef_AniDB_TvDBV2 contract in details.CrossRef_AniDB_TvDB)
				{
					CrossRef_AniDB_TvDBVMV2 xref = new CrossRef_AniDB_TvDBVMV2(contract);
					CrossRef_AniDB_TvDB.Add(xref);
				}
			}

			if (details.TvDBSeries != null)
			{
				TvDBSeries = new List<TvDB_SeriesVM>();
				foreach (JMMServerBinary.Contract_TvDB_Series contract in details.TvDBSeries)
				{
					TvDB_SeriesVM tv = new TvDB_SeriesVM(contract);
					TvDBSeries.Add(tv);
				}
			}

			foreach (JMMServerBinary.Contract_TvDB_Episode contract in details.TvDBEpisodes)
				TvDBEpisodes.Add(new TvDB_EpisodeVM(contract));

			foreach (JMMServerBinary.Contract_TvDB_ImageFanart contract in details.TvDBImageFanarts)
			{
				bool isDefault = false;
				if (anime != null && anime.DefaultFanart != null && anime.DefaultFanart.ImageParentType == (int)ImageEntityType.TvDB_FanArt
					&& anime.DefaultFanart.ImageParentID == contract.TvDB_ImageFanartID)
				{
					isDefault = true;
				}

				TvDB_ImageFanartVM tvFanart = new TvDB_ImageFanartVM(contract);
				tvFanart.IsImageDefault = isDefault;
				TvDBImageFanarts.Add(tvFanart);

				AllFanarts.Add(new FanartContainer(ImageEntityType.TvDB_FanArt, tvFanart));
			}

			foreach (JMMServerBinary.Contract_TvDB_ImagePoster contract in details.TvDBImagePosters)
			{
				bool isDefault = false;
				if (anime != null && anime.DefaultPoster != null && anime.DefaultPoster.ImageParentType == (int)ImageEntityType.TvDB_Cover
					&& anime.DefaultPoster.ImageParentID == contract.TvDB_ImagePosterID)
				{
					isDefault = true;
				}

				TvDB_ImagePosterVM tvPoster = new TvDB_ImagePosterVM(contract);
				tvPoster.IsImageDefault = isDefault;
				TvDBImagePosters.Add(tvPoster);
				AllPosters.Add(new PosterContainer(ImageEntityType.TvDB_Cover, tvPoster));
			}

			foreach (JMMServerBinary.Contract_TvDB_ImageWideBanner contract in details.TvDBImageWideBanners)
			{
				bool isDefault = false;
				if (anime != null && anime.DefaultWideBanner != null && anime.DefaultWideBanner.ImageParentType == (int)ImageEntityType.TvDB_Banner
					&& anime.DefaultWideBanner.ImageParentID == contract.TvDB_ImageWideBannerID)
				{
					isDefault = true;
				}

				TvDB_ImageWideBannerVM tvBanner = new TvDB_ImageWideBannerVM(contract);
				tvBanner.IsImageDefault = isDefault;
				TvDBImageWideBanners.Add(tvBanner);
			}

			if ((CrossRef_AniDB_TvDB == null || CrossRef_AniDB_TvDB.Count ==0) || 
				(TvDBSeries == null || TvDBSeries.Count == 0))
				TvDBCrossRefExists = false;
			else
				TvDBCrossRefExists = true;

			// MovieDB
			if (details.CrossRef_AniDB_MovieDB != null)
				CrossRef_AniDB_MovieDB = new CrossRef_AniDB_OtherVM(details.CrossRef_AniDB_MovieDB);

			if (details.MovieDBMovie != null)
				MovieDB_Movie = new MovieDB_MovieVM(details.MovieDBMovie);

			foreach (JMMServerBinary.Contract_MovieDB_Fanart contract in details.MovieDBFanarts)
			{
				bool isDefault = false;
				if (anime != null && anime.DefaultFanart != null && anime.DefaultFanart.ImageParentType == (int)ImageEntityType.MovieDB_FanArt
					&& anime.DefaultFanart.ImageParentID == contract.MovieDB_FanartID)
				{
					isDefault = true;
				}

				MovieDB_FanartVM movieFanart = new MovieDB_FanartVM(contract);
				movieFanart.IsImageDefault = isDefault;
				MovieDBFanarts.Add(movieFanart);
				AllFanarts.Add(new FanartContainer(ImageEntityType.MovieDB_FanArt, movieFanart));
			}

			foreach (JMMServerBinary.Contract_MovieDB_Poster contract in details.MovieDBPosters)
			{
				bool isDefault = false;
				if (anime != null && anime.DefaultPoster != null && anime.DefaultPoster.ImageParentType == (int)ImageEntityType.MovieDB_Poster
					&& anime.DefaultPoster.ImageParentID == contract.MovieDB_PosterID)
				{
					isDefault = true;
				}

				MovieDB_PosterVM moviePoster = new MovieDB_PosterVM(contract);
				moviePoster.IsImageDefault = isDefault;
				MovieDBPosters.Add(moviePoster);
				AllPosters.Add(new PosterContainer(ImageEntityType.MovieDB_Poster, moviePoster));
			}

			if (CrossRef_AniDB_MovieDB == null || MovieDB_Movie == null)
				MovieDBCrossRefExists = false;
			else
				MovieDBCrossRefExists = true;

		}
	}
}
