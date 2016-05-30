using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

using MusicPlayer;
using MusicPlayer.Api;
using MusicPlayer.Data;
using MusicPlayer.Managers;
using MusicPlayer.Models;
using MusicPlayer.Models.Scrobbling;

namespace TunezApi
{
	public partial class TunezProvider : MusicProvider
	{
		static readonly string CatalogCache = System.IO.Path.Combine (Locations.LibDir, "tunezprovider.catalog");

		SimpleAuth.AuthenticatedApi Api {
			get;
		}

		public override MediaProviderCapabilities[] Capabilities {
			get { return new [] { MediaProviderCapabilities.None }; }
		}

		public override string Email {
			get { return Api.BaseAddress.ToString (); }
		}

		public override string Id {
			get { return Api.CurrentAccount?.Identifier; }
		}

		public override bool RequiresAuthentication {
			get { return false; }
		}

		public Tunez.TunezServer Server {
			get;
		}

		public override ServiceType ServiceType {
			get { return ServiceType.Tunez; }
		}

		public TunezProvider (SimpleAuth.AuthenticatedApi api)
			: base (api)
		{
			Api = api;
			Server = new Tunez.TunezServer (new Tunez.ServerDetails {
				Hostname = Api.BaseAddress.Host,
				Port = api.BaseAddress.Port,
			});
		}

		public async override Task<DownloadUrlData> GetDownloadUri(Track track)
		{
			var playbackUri = await GetPlaybackUri (track);
			var data = new DownloadUrlData {
				Url = playbackUri.OriginalString
			};
			return data;
		}

		public override Task<Uri> GetPlaybackUri(Track track)
		{
			var fetchTrackMessage = new Tunez.FetchTrackMessage { UUID = int.Parse (track.Id), Offset = 0 };
			var message = Tunez.Messages.FetchTrack + Newtonsoft.Json.JsonConvert.SerializeObject (fetchTrackMessage);

			return Task.FromResult (new Uri (Api.BaseAddress, "?" + Uri.EscapeUriString (message)));
		}

		public override Task<bool> Resync()
		{
			return SyncDatabase ();
		}

		protected async override Task<bool> Sync()
		{
			var catalog = await Server.FetchCatalog (CatalogCache, System.Threading.CancellationToken.None);
			var tracks = catalog.Select (t => { 
				var track = new FullTrackData (t.Name, t.TrackArtist, t.AlbumArtist, t.Album, "Genre") {
					Duration = t.Duration,
					Disc = t.Disc,
					FileExtension = "mp3",
					MediaType = MediaType.Audio,
					Track = t.Number,
					Id = t.UUID.ToString (),
					ServiceId = Id,
				};

				if (t.AlbumArt != null) {
					var fetchAlbumArtMessage = new Tunez.FetchAlbumArtMessage {
						AlbumArtist = t.AlbumArtist,
						Album = t.Album,
					};
					var message = Tunez.Messages.FetchAlbumArt + Newtonsoft.Json.JsonConvert.SerializeObject (fetchAlbumArtMessage);
					track.AlbumArtwork = new List<AlbumArtwork> {
						new AlbumArtwork { Url = Api.BaseAddress + "?" + Uri.EscapeUriString (message) }
					};

					var fetchArtistArtMessage = new Tunez.FetchArtistArtMessage {
						AlbumArtist = t.AlbumArtist,
					};
					message = Tunez.Messages.FetchArtistArt + Newtonsoft.Json.JsonConvert.SerializeObject (fetchArtistArtMessage);
					track.ArtistArtwork = new List<ArtistArtwork> {
						new ArtistArtwork { Url = Api.BaseAddress + "?" + Uri.EscapeUriString (message) }
					};
				}
				return track;
			}).ToList ();

			await ProcessTracks (tracks);
			await FinalizeProcessing (Id);
			MusicPlayer.Managers.ApiManager.Shared.SaveApi(Api);

			return true;
		}

		public override Task<bool> AddToLibrary(OnlineAlbum album)
		{
			throw new NotImplementedException();
		}
		public override Task<bool> AddToLibrary(Track track)
		{
			throw new NotImplementedException();
		}
		public override Task<bool> AddToLibrary(OnlineSong song)
		{
			throw new NotImplementedException();
		}
		public override Task<bool> AddToLibrary(OnlinePlaylist playlist)
		{
			throw new NotImplementedException();
		}
		public override Task<bool> AddToPlaylist(List<Track> songs, string playlistName)
		{
			throw new NotImplementedException();
		}
		public override Task<bool> AddToPlaylist(List<Track> songs, Playlist playlist)
		{
			throw new NotImplementedException();
		}
		public override Task<bool> DeletePlaylist(Playlist playlist)
		{
			throw new NotImplementedException();
		}
		public override Task<bool> DeletePlaylistSong(PlaylistSong song)
		{
			throw new NotImplementedException();
		}
		public override Task<List<Song>> GetAlbumDetails(string id)
		{
			throw new NotImplementedException();
		}
		public override Task<SearchResults> GetArtistDetails(string id)
		{
			throw new NotImplementedException();
		}
		public override Task<List<OnlinePlaylistEntry>> GetPlaylistEntries(OnlinePlaylist playlist)
		{
			throw new NotImplementedException();
		}
		public override Task<bool> SetRating(Track track, int rating)
		{
			throw new NotImplementedException();
		}
		public override Task<string> GetShareUrl(Song song)
		{
			throw new NotImplementedException();
		}

		public override async Task Logout()
		{
			ApiManager.Shared.DeleteApi(Api);
			await RemoveApi(Id);
			Api.ResetData();
		}

		public override Task<bool> MoveSong(PlaylistSong song, string previousId, string nextId, int index)
		{
			throw new NotImplementedException();
		}
		public override Task<bool> RecordPlayack(PlaybackEndedEvent data)
		{
			return Task.FromResult (true);
		}
		public override Task<SearchResults> Search(string query)
		{
			throw new NotImplementedException();
		}
	}
}

