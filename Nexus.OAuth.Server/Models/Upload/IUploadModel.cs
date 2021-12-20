namespace Nexus.OAuth.Server.Models.Upload;

internal interface IUploadModel<T>
{
    /// <summary>
    /// Transform this model object in database model Object
    /// </summary>
    /// <returns>Returns data model object</returns>
    public T ToDataModel();
}

