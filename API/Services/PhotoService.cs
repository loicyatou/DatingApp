using API.Helpers;
using CloudinaryDotNet;
using CloudinaryDotNet.Actions;
using Microsoft.Extensions.Options;

namespace API;

public class PhotoService : IPhotoService
{
    private readonly Cloudinary _cloudinary;

    public PhotoService(IOptions<CloudinarySettings> config)
    {
        //Cloudinary provides schema to inject config settings into an instance
        var acc = new Account
        (
            config.Value.CloudName,
            config.Value.ApiKey,
            config.Value.ApiSecret
        );

        _cloudinary = new Cloudinary(acc);
    }
    public async Task<ImageUploadResult> AddPhotoAsync(IFormFile file)
    {
        var uploadResult = new ImageUploadResult();

        if(file.Length > 0){
            using var stream = file.OpenReadStream(); //using: so that it closes the stream once it is complete to save resources
            var uploadParams = new ImageUploadParams //provided by cloudinary
            {
                File = new FileDescription(file.FileName, stream),
                Transformation = new Transformation().Height(500).Width(500).Crop("fill").Gravity("face"), //adds constraints on images user upload. auto crop image to a square and gravity will focus crop on face. 
                Folder = "da-net7"
            };
            uploadResult = await _cloudinary.UploadAsync(uploadParams);
        }
        return uploadResult; //object representing details about the upload
    }

    public async Task<DeletionResult> DeletePhotoAsync(string publicID)
    {
        var deleteParams = new DeletionParams(publicID);
        return await _cloudinary.DestroyAsync(deleteParams); //locate image id and remove it. also functions provided by cloudinary
    }
}
