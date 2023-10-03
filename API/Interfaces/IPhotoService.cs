using CloudinaryDotNet.Actions;

namespace API;

public interface IPhotoService
{
Task<ImageUploadResult> AddPhotoAsync(IFormFile file); //Iform is a datatype to rep file in http request 
Task<DeletionResult> DeletePhotoAsync(string publicID);


}
