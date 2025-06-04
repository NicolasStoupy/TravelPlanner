using BussinessLogic.Entities;
using QuestPDF.Fluent;
using QuestPDF.Helpers;
using QuestPDF.Infrastructure;


namespace BussinessLogic.Models
{   /// <summary>
    /// Generates a PDF document representation of a Travel entity, including metadata, overview, statistics, activities, memories, and notes.
    /// </summary>
    public class TravelDocumentPDF (Travel travel) : IDocument
    {
       
      
        private readonly Travel _travel = travel;
        public DocumentMetadata GetMetadata() => DocumentMetadata.Default;

        /// <summary>
        /// Initializes a new instance of the <see cref="TravelDocumentPDF"/> class using the provided travel data.
        /// </summary>
        /// <param name="travel">The <see cref="Travel"/> entity to generate the PDF for.</param>
        /// <exception cref="ArgumentNullException">Thrown if <paramref name="travel"/> is null.</exception>
        public void Compose(IDocumentContainer container)
        {
            

            container.Page(page =>
            {
                page.Size(PageSizes.A4);
                page.Margin(2, Unit.Centimetre);
                page.DefaultTextStyle(x => x.FontSize(12));
                page.Content().Column(col =>
                {
                      // Titre et description
                     col.Item().PaddingTop(10).Text(_travel.name).FontSize(20).Bold();
                    col.Item().Text(_travel.description ?? "Aucune description.");
                    // Image de couverture
                    if (_travel.image is { Length: > 0 })
                    {
                        col.Item().Image(_travel.image, ImageScaling.FitWidth);
                    }
                    else
                    {
                        col.Item().Text("[Aucune image disponible]").Italic().FontSize(10);
                    }

                

                    // Dates
                    col.Item().PaddingTop(5).Text($"Du {_travel.StartDate:d} au {_travel.EndDate:d}");

                    // Participants
                    col.Item().PaddingTop(5).Text("Participants : " + _travel.people.ToString());

                    // Statistiques en bulles
                    col.Item().PaddingVertical(10).Row(row =>
                    {
                        void StatBox(string label, string value)
                        {
                            row.ConstantItem(100).Column(col2 =>
                            {
                                col2.Item().Background(Colors.Blue.Lighten2).Padding(10).AlignCenter().Text(value).FontSize(16).Bold().FontColor(Colors.White);
                                col2.Item().AlignCenter().Text(label);
                            });
                        }

                        StatBox("Activités", _travel.CountActivities.ToString());
                        StatBox("Souvenirs", _travel.MemoryFiles.Count.ToString());
                        StatBox("Notes", _travel.CountNote.ToString());
                        StatBox("Budget", $"{_travel.budget:F1} €");
                     
                    });
                    col.Item().PageBreak();
                    // === Détail des activités ===
                    if (_travel.TravelActivities?.Any() == true)
                    {
                        col.Item().PaddingTop(20).Text("Détail des activités :").FontSize(14).Bold();

                        foreach (var activity in _travel.TravelActivities.OrderBy(a => a.Sequence))
                        {
                            col.Item().PaddingVertical(5).BorderBottom(1).Column(act =>
                            {
                                act.Item().Text($"#{activity.Sequence} : {activity.Name}").FontSize(12).Bold();

                                act.Item().Text($"{activity.Name} | {activity.ActivityDate:d} | Coût Planifié: {activity.PlannedCost:F2} €")
                                          .FontSize(10).Italic().FontColor(Colors.Grey.Darken1);

                                if (!string.IsNullOrWhiteSpace(activity.Description))
                                {
                                    act.Item().Text(activity.Description).FontSize(10);
                                }



                                //if (activity.Costs?.Any() == true)
                                //{
                                //    var totalCost = activity.Costs.Sum(c => (decimal?)c.Amount) ?? 0;
                                //    act.Item().Text($"Coût total : {totalCost:F2} €").FontSize(10).FontColor(Colors.Blue.Darken2);
                                //}
                            });
                        }
                    }
                    col.Item().PageBreak();
                    // Souvenirs (photos)
                    if (_travel.MemoryFiles.Any(m => m.Files != null && m.Files.Length > 0))
                    {
                        col.Item().PaddingTop(10).Text("Souvenirs en image :").Bold();

                        var images = _travel.MemoryFiles.Where(m => m.Files != null && m.Files.Length > 0).ToList();

                        col.Item().Grid(grid =>
                        {
                            grid.Columns(3); // 3 images par ligne

                            foreach (var memory in images)
                            {
                                grid.Item().Padding(5).Column(col2 =>
                                {
                                    col2.Item().Image(memory.Files, ImageScaling.FitWidth);
                                    col2.Item().Text(memory.Description ?? "").FontSize(10).Italic();
                                });
                            }
                        });
                    }                  
                  
                    col.Item().PageBreak();
                    if (_travel.TravelNotes.Any())
                    {
                        col.Item().PaddingTop(10).Text("Mes Notes Voyages :").Bold();
                        var notes = _travel.TravelNotes.ToList();
                        col.Item().Grid(grid =>
                        {
                            grid.Columns(3); // 3 images par ligne

                            foreach (var note in notes)
                            {
                                grid.Item().Padding(5).Column(col2 =>
                                {

                                    col2.Item().Text(note.NoteContent ?? "").FontSize(10).Italic();
                                });
                            }
                        });
                    }
                });
            });
        }
    }
}
