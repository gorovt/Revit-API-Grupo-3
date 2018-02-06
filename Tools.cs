// Variables necesarias en todos los Comandos Externos
UIApplication uiApp = commandData.Application;
UIDocument uiDoc = uiApp.ActiveUIDocument;
Application app = uiApp.Application;
Document doc = uiDoc.Document;

// Obtener la referencia de un objeto seleccionado
Reference r = uiDoc.Selection.PickObject(Autodesk.Revit.UI.Selection.ObjectType.Element, "Seleccionar un elemento");
// Obtener el elemento elegido
Element e = doc.GetElement(r.ElementId);

// Obtener Parametros
Parameter pComentario = _elem.get_Parameter(BuiltInParameter.ALL_MODEL_INSTANCE_COMMENTS);
Parameter pMarca = _elem.get_Parameter(BuiltInParameter.DOOR_NUMBER);

// Iniciar Nueva Transaccion
Transaction t = new Transaction(_doc, "Cambiar Parametro");
t.Start();

// Modificar Parametros
pComentario.Set(comentario);
pMarca.Set(marca);

t.Commit();

// Filtros y Colecciones
// Encontrar todos los ejemplares de Muros del Documento
ElementCategoryFilter filter = new ElementCategoryFilter(BuiltInCategory.OST_Walls);
FilteredElementCollector collector = new FilteredElementCollector(document);
List<Element> lista = collector.WherePasses(filter).WhereElementIsNotElementType().ToList();
// Otras variantes
// Obtener todas las Topografías
FilteredElementCollector collector = new FilteredElementCollector(doc);
List<Element> lstTopo = collector.OfClass(typeof(TopographySurface)).ToList();

// LinQ: se necesita igualmente un Collector
FilteredElementCollector collector = new FilteredElementCollector(doc);
List<Element> elements = collector.WhereElementIsNotElementType().ToList();
List<Element> lstInstance = (from elem in elements
                                         where elem.Category != null
                                         && elem.Category.Id != (new ElementId(BuiltInCategory.OST_Cameras))
                                         && elem.Category.Id != (new ElementId(BuiltInCategory.OST_StackedWalls))
                                         select elem).ToList();

// Tools
/// <summary> Obtener una Lista de elementos según una Categoría determinada </summary>
public static List<Element> ObtenerElementosPorCategoria(Document doc, ElementId categoriaId)
{
    FilteredElementCollector colector = new FilteredElementCollector(doc);
    List<Element> elementos = colector.WhereElementIsNotElementType().ToList();
    List<Element> filtrados = (from elem in elementos
                               where elem.Category != null
                               && elem.Category.Id == categoriaId
                               select elem).ToList();

    return filtrados;
}

/// <summary> Obtiene una Lista de Categorias existentes en el Modelo </summary>
public static List<Category> ObtenerCategoriasModelo(Document doc)
{
    FilteredElementCollector colector = new FilteredElementCollector(doc);
    List<Element> elementos = colector.WhereElementIsNotElementType().ToList();
    List<Element> filtrados = (from elem in elementos
                               where elem.Category != null
                               && elem.Category.CategoryType == CategoryType.Model
                               select elem).ToList();

    // Lista vacias de categorias
    List<Category> categorias = new List<Category>();
    // Recorrer los elementos filtrados
    foreach (Element elem in filtrados)
    {
        // Validar que la categoria NO exista en la lista
        if (!categorias.Exists(x => x.Id == elem.Category.Id))
        {
            categorias.Add(elem.Category);
        }
    }
    return categorias;
}
// Seleccionar elemento en la Interfaz
List<ElementId> lista = new List<ElementId>();
lista.Add(elem.Id);
uiDoc.Selection.SetElementIds(lista);

// Obtener todos los Elementos del Modelo
public static List<Element> GetAllElements(Document doc)
{
    FilteredElementCollector collector = new FilteredElementCollector(doc);
    return collector.WhereElementIsNotElementType().ToList();
}

// Crear una lista de Niveles del Proyecto
public static List<Level> ObtenerListaNiveles(Document doc)
{
    // Crear una lista vacia de niveles
    List<Level> niveles = new List<Level>();

    // Crear un collector de Niveles
    FilteredElementCollector collector = new FilteredElementCollector(doc);
    List<Element> elementos = collector.OfClass(typeof(Level)).ToList();

    // Llenar la lista de Niveles
    foreach (var item in elementos)
    {
        // Convierto el Elemento en un Nivel
        Level nivel = item as Level;

        // Lleno la lista de niveles
        niveles.Add(nivel);
    }
    return niveles;
}
// Obtener una Lista de FamilySymbol
public static List<FamilySymbol> ObtenerListaTiposFamiliaModelo(Document doc)
{
    List<FamilySymbol> lst = new List<FamilySymbol>();
    FilteredElementCollector collector = new FilteredElementCollector(doc);
    List<Element> lstElem = collector.OfClass(typeof(FamilySymbol)).ToList();
    foreach (Element elem in lstElem)
    {
        // Convertir el Elemento en FamilySymbol
        FamilySymbol fm = elem as FamilySymbol;
        // Verificar si es una Familia de Modelo
        if (fm.Category.CategoryType == CategoryType.Model)
        {
            lst.Add(fm);
        }
    }
    return lst;
}

// Crear una lista de Categorias del Modelo
public static List<Category> ObtenerListaCategoriasModelo(Document doc)
{
    // Crear una lista vacia
    List<Category> categorias = new List<Category>();

    // Obtener una lista de tipos de familia cargados
    List<FamilySymbol> tipos = ObtenerListaTiposFamiliaModelo(doc);

    // Rellenar la Lista de Category
    foreach (var item in tipos)
    {
        // Verificar que la Categoria NO exista en la Lista
        if (!categorias.Exists(x => x.Name == item.Category.Name))
        {
            categorias.Add(item.Category);
        }
    }
    return categorias;
}

// Obtener una Familia a partir de su nombre
public static FamilySymbol ObtenerTipoFamiliaPorNombre(Document doc, string name)
{
    FamilySymbol family = null;
    foreach (FamilySymbol sym in GetAllFamilySymbol(doc))
    {
        if (sym.Name == name)
        {
            family = sym;
        }
    }
    return family;
}

// Obtener un Nivel a partir de su nombre
public static Level ObtenerNivelPorNombre(Document doc, string name)
{
    Level lvl = null;
    foreach (Level level in GetAllLevels(doc))
    {
        if (level.Name == name)
        {
            lvl = level;
        }
    }
    return lvl;
}

// Crear Ejemplares de Familia. Se debe crear una lista de FamilyInstanceCreationData
// Se debe referenciar <<using Autodesk.Revit.Creation;>>
FamilyInstanceCreationData ficreationdata = new FamilyInstanceCreationData(pointXYZ, familySymbol, 
                        level, Autodesk.Revit.DB.Structure.StructuralType.NonStructural);
_doc.Create.NewFamilyInstances2(lstData);
