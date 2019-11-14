# PodcastCore.MediaInfoServer

Provides access to the properties of audio files, like container format and duration.

It uses [MediaInfo](https://mediaarea.net/en/MediaInfo) to read such properties from files.

The API endpoints accepts object keys used to fetch audio files from the blob storage (e.g AWS S3 or Minio), then returns requested property information.  
Directly uploading audio files to the endpoint is also supported.

## TODOs

- [X] Implement object fetching endpoint
- [ ] Implement direct uploading endpoint
- [ ] Support of DLL mapping to load MediaInfo assemblies in other platforms

## License

MIT
