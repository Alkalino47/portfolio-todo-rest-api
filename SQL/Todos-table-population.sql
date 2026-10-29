USE TodosDB;
GO

INSERT INTO [dbo].[Todos]
    ([Name], [Description], [IsComplete], [DatetimeReminder], [CategoryId], [UserId], [DateCreated], [DateModified], [IsArchived], [DateArchived])
VALUES
    ('Complete REST API Todo', 'Complete all activities to finish REST API to manage todos', 0, NULL, 1001, 1, '2026-06-24 10:00:00.000', '2026-06-24 10:00:00.000', 0, NULL),
    ('Review clean architecture', 'Check the API, Application, Domain, and Infrastructure layer responsibilities', 0, NULL, 1001, 1, '2026-06-24 10:05:00.000', '2026-06-24 10:05:00.000', 0, NULL),
    ('Buy groceries', 'Milk, bread, coffee, and fruit', 0, '2026-06-25 18:00:00.000', 1003, 1, '2026-06-24 10:10:00.000', '2026-06-24 10:10:00.000', 0, NULL),
    ('Archive old task', 'Example archived todo item', 1, NULL, 1001, 1, '2026-06-24 10:15:00.000', '2026-06-24 10:30:00.000', 1, '2026-06-24 10:30:00.000'),
    ('Set reminder', 'Prepare for tomorrow morning planning session', 0, '2026-06-25 09:00:00.000', 1002, 1, '2026-06-24 10:20:00.000', '2026-06-24 10:20:00.000', 0, NULL);
