using System.Reflection;

namespace Nexus.OAuth.Api.Models.Upload;

/// <summary>
/// Base class of models using in upload
/// </summary>
/// <typeparam name="T">Type of Microsoft.EntityFrameworkCore.DbSet`1 class. </typeparam>
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
    internal abstract void UpdateModel(in T model);

    /// <summary>
    /// Updates properties with the same type and name as the model object in the database using the properties of an upload type model.
    /// </summary>
    /// <typeparam name="Tclass">Super class type.</typeparam>
    /// <param name="data">Data model instance.</param>
    protected internal void UpdateModel<Tclass>(in T data)
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

            object? thisValue = thisProperty.GetValue(this);
            object? dataValue = dataProperty.GetValue(data);

            if (CheckPropertyUpdate(dataValue, thisValue))
            {
                if (dataProperty.PropertyType == thisProperty.PropertyType)
                {
                    dataProperty.SetValue(data, thisValue);
                }
            }
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
        var updated =
                      prop == null &&
                      comparetor != null;

        if (comparetor != null && prop != null)
        {
            if (!comparetor.Equals(prop))
                updated = true;
        }

        return updated;
    }
}

