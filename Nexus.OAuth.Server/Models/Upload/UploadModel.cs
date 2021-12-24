using System.Reflection;

namespace Nexus.OAuth.Server.Models.Upload;

public abstract class UploadModel<T>
{
    /// <summary>
    /// Transform this model object in database model Object
    /// </summary>
    /// <returns>Returns data model object</returns>
    public abstract T ToDataModel();
    /// <summary>
    /// Update database model using this model
    /// </summary>
    /// <param name="model">Database model reference</param>
    public abstract void UpdateModel(ref T model);

    /// <summary>
    /// 
    /// </summary>
    /// <typeparam name="Tclass">Super class type.</typeparam>
    /// <param name="data">Data model instance.</param>
    public void UpdateModel<Tclass>(ref T data)
    {
        Type dataModelType = typeof(T);
        Type thisType = typeof(Tclass);

        PropertyInfo[] dataProperties = dataModelType.GetProperties();
        PropertyInfo[] thisProperties = thisType.GetProperties();

        foreach (PropertyInfo dataProperty in dataProperties)
        {
            PropertyInfo? thisProperty = thisProperties
                                            .FirstOrDefault(fs => fs.Name == dataProperty.Name);

            if (thisProperty == null)
                continue;

           // TODO: 
        }
    }

    /// <summary>
    /// Checks whether the value of the input model property is updated compared to the database model.
    /// </summary>
    /// <param name="prop">Value data model propertie.</param>
    /// <param name="comparetor">Value of this class propertie.</param>
    /// <returns>
    ///          TRUE: Indicates that the input model had the field updated compared to the database model.
    ///          FALSE: Indicates that the upload model propertie is not modified.
    /// </returns>
    protected internal virtual bool CheckPropertyUpdate(object? prop, object? comparetor)
    {
        bool updated = false;

        if (prop == null && comparetor != null)
            updated = true;

        if (comparetor != null && prop != null)
        {
            if (!comparetor.Equals(prop))
                updated = true;
        }

        return updated;
    }
}

