﻿using FluentAssertions;

using Mongo.Migration.Test.TestDoubles;

using MongoDB.Bson;

using NUnit.Framework;

namespace Mongo.Migration.Test.Migrations.Document;

[TestFixture]
public class DocumentMigration_when_migrating
{
    [Test]
    public void When_migrating_down_Then_document_changes()
    {
        // Arrange
        var migration = new TestDocumentWithOneMigration_0_0_1();
        var document = new BsonDocument { { "Doors", 3 } };

        // Act
        migration.Down(document);

        // Assert
        document.Should().BeEquivalentTo(new BsonDocument { { "Dors", 3 } });
    }

    [Test]
    public void When_migrating_up_Then_document_changes()
    {
        // Arrange
        var migration = new TestDocumentWithOneMigration_0_0_1();
        var document = new BsonDocument { { "Dors", 3 } };

        // Act
        migration.Up(document);

        // Assert
        document.Should().BeEquivalentTo(new BsonDocument { { "Doors", 3 } });
    }
}