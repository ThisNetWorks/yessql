using Parlot.Fluent;
using System;
using System.Linq;
using Xunit;
using YesSql.Services;
using YesSql.Tests.Indexes;
using YesSql.Tests.Models;
using static YesSql.Core.QueryParser.Fluent.QueryParsers;

namespace YesSql.Tests.QueryParserTests
{
    public class SearchParserTests
    {

        [Fact]
        public void ShouldParseNamedTerm()
        {
            var parser = QueryParser(
                NamedTermParser("name",
                    OneConditionParser(PersonOneConditionQuery())
                )
            );

            Assert.Equal("name:steve", parser.Parse("name:steve").ToString());
            Assert.Equal("name:steve", parser.Parse("name:steve").ToNormalizedString());
        }

        [Fact]
        public void ShouldParseManyNamedTerms()
        {
            var parser = QueryParser(
                NamedTermParser("name",
                    OneConditionParser<Person>(PersonOneConditionQuery())
                ),
                NamedTermParser("status",
                    OneConditionParser<Person>(PersonOneConditionQuery())
                )
            );

            Assert.Equal("name:steve status:published", parser.Parse("name:steve status:published").ToString());
            Assert.Equal("name:steve status:published", parser.Parse("name:steve status:published").ToNormalizedString());
        }

        [Fact]
        public void ShouldParseManyNamedTermsWithManyCondition()
        {
            var parser = QueryParser(
                NamedTermParser("name",
                    ManyConditionParser<Person>(PersonManyMatch(), PersonManyNotMatch())
                ),
                NamedTermParser("status",
                    ManyConditionParser<Person>(PersonManyMatch(), PersonManyNotMatch())
                )
            );

            Assert.Equal("name:steve status:published", parser.Parse("name:steve status:published").ToString());
            Assert.Equal("name:steve status:published", parser.Parse("name:steve status:published").ToNormalizedString());
        }

        [Fact]
        public void ShouldParseDefaultTermWithManyCondition()
        {
            var parser = QueryParser(
                DefaultTermParser("name",
                    ManyConditionParser<Person>(PersonManyMatch(), PersonManyNotMatch())
                ),
                NamedTermParser("status",
                    ManyConditionParser<Person>(PersonManyMatch(), PersonManyNotMatch())
                )
            );

            Assert.Equal("steve status:published", parser.Parse("steve status:published").ToString());
            Assert.Equal("name:steve status:published", parser.Parse("steve status:published").ToNormalizedString());
        }

        [Fact]
        public void ShouldParseDefaultTermWithManyConditionWhenLast()
        {
            var parser = QueryParser(
                NamedTermParser("status",
                    ManyConditionParser<Person>(PersonManyMatch(), PersonManyNotMatch())
                ),
                DefaultTermParser("name",
                    ManyConditionParser<Person>(PersonManyMatch(), PersonManyNotMatch())
                )
            );

            Assert.Equal("steve status:published", parser.Parse("steve status:published").ToString());
            Assert.Equal("name:steve status:published", parser.Parse("steve status:published").ToNormalizedString());
        }

        [Fact]
        public void ShouldParseDefaultTermWithManyConditionWhenDefaultIsLast()
        {
            // TODO we just need a validation to stop this happening.
            // so really the answer is if you have two manys. you cannot have a default.
            var parser = QueryParser(
                DefaultTermParser("name",
                    ManyConditionParser<Person>(PersonManyMatch(), PersonManyNotMatch())
                ),
                NamedTermParser("status",
                    ManyConditionParser<Person>(PersonManyMatch(), PersonManyNotMatch())
                )
            );

            // Ah I see. output is status:(published OR steve)
            // so it doesn't matter what order they are registered in
            // but somehow we might need to get the default one to run first, and see what if can find.

            Assert.Equal("status:(published OR steve)", parser.Parse("status:published steve").ToNormalizedString());
        }

        [Fact]
        public void ShouldParseDefaultTerm()
        {
            var parser = QueryParser(
                NamedTermParser("age",
                    OneConditionParser<Person>(PersonOneConditionQuery())
                ),
                DefaultTermParser("name",
                    OneConditionParser<Person>(PersonOneConditionQuery())
                )
            );

            Assert.Equal("name:steve", parser.Parse("name:steve").ToString());
            Assert.Equal("steve", parser.Parse("steve").ToString());
            Assert.Equal("steve age:20", parser.Parse("steve age:20").ToString());
            Assert.Equal("age:20 name:steve", parser.Parse("age:20 name:steve").ToString());
            Assert.Equal("age:20 steve", parser.Parse("age:20 steve").ToString());
            Assert.Equal(2, parser.Parse("steve age:20").Terms.Count());
            Assert.Equal("name:steve", parser.Parse("steve").ToNormalizedString());
        }

        [Fact]
        public void ShouldParseDefaultTermWithOneMany()
        {
            var parser = QueryParser(
                NamedTermParser("age",
                    OneConditionParser<Person>(PersonOneConditionQuery())
                ),
                DefaultTermParser("name",
                    ManyConditionParser<Person>(PersonManyMatch(), PersonManyNotMatch())
                )
            );

            Assert.Equal("name:steve", parser.Parse("name:steve").ToString());
            Assert.Equal("steve", parser.Parse("steve").ToString());
            Assert.Equal("steve age:20", parser.Parse("steve age:20").ToString());
            Assert.Equal("age:20 name:steve", parser.Parse("age:20 name:steve").ToString());
            Assert.Equal("age:20 steve", parser.Parse("age:20 steve").ToString());
            Assert.Equal(2, parser.Parse("steve age:20").Terms.Count());
            Assert.Equal("name:steve", parser.Parse("steve").ToNormalizedString());
        }

        [Fact]
        public void ShouldParseDefaultTermAtEndOfStatement()
        {
            var parser = QueryParser(
                NamedTermParser("age",
                    OneConditionParser<Person>((val, query) =>
                    {
                        if (Int32.TryParse(val, out var age))
                        {
                            query.With<PersonByAge>(x => x.Age == age);
                        }

                        return query;
                    })
                ),
                DefaultTermParser("name",
                    OneConditionParser<Person>(PersonOneConditionQuery())
                )
            );

            Assert.Equal("age:20 name:steve", parser.Parse("age:20 name:steve").ToString());
            Assert.Equal(2, parser.Parse("age:20 name:steve").Terms.Count());
            Assert.Equal("age:20 steve", parser.Parse("age:20 steve").ToString());
            Assert.Equal(2, parser.Parse("age:20 steve").Terms.Count());
        }

        [Fact]
        public void OrderOfDefaultTermShouldNotMatter()
        {
            var namedParser = NamedTermParser("age",
                OneConditionParser<Person>(PersonOneConditionQuery()) // TODO this is failing when it's a Many
            );

            var defaultParser = DefaultTermParser("name",
                ManyConditionParser<Person>(PersonManyMatch(), PersonManyNotMatch())
            );

            var parser1 = QueryParser(
                namedParser,
                defaultParser
             );

            var parser2 = QueryParser(
                 defaultParser,
                 namedParser
             );
            // sand status:published is returning 1 when it should return 2. it's the same as parser1.
            Assert.Equal("steve age:20", parser1.Parse("steve age:20").ToString());

            var result = parser1.Parse("steve age:20");
            Assert.Equal(2, result.Terms.Count());

            Assert.Equal("age:20 steve", parser1.Parse("age:20 steve").ToString());
            Assert.Equal(2, parser1.Parse("age:20 steve").Terms.Count());

            Assert.Equal("steve age:20", parser2.Parse("steve age:20").ToString());
            Assert.Equal(2, parser2.Parse("steve age:20").Terms.Count());

            Assert.Equal("age:20 steve", parser2.Parse("age:20 steve").ToString());
            Assert.Equal(2, parser2.Parse("age:20 steve").Terms.Count());
        }

        [Theory]
        [InlineData("title:bill post", "title:(bill OR post)")]
        [InlineData("title:bill OR post", "title:(bill OR post)")]
        [InlineData("title:beach AND sand", "title:(beach AND sand)")]
        [InlineData("title:beach AND sand OR mountain AND lake", "title:((beach AND sand) OR (mountain AND lake))")]
        [InlineData("title:(beach AND sand) OR (mountain AND lake)", "title:((beach AND sand) OR (mountain AND lake))")]
        [InlineData("title:(beach AND sand) OR (mountain AND lake) NOT lizards", "title:(((beach AND sand) OR (mountain AND lake)) NOT lizards)")]

        [InlineData("title:NOT beach", "title:NOT beach")]
        [InlineData("title:beach NOT mountain", "title:(beach NOT mountain)")]
        [InlineData("title:beach NOT mountain lake", "title:((beach NOT mountain) OR lake)")] // this is questionable, but with the right () can achieve anything
        public void Complex(string search, string normalized)
        {
            var parser = QueryParser(
                NamedTermParser("title",
                    ManyConditionParser<Article>(
                        ArticleManyMatch(),
                        ArticleManyNotMatch()
                    )
                )
            );

            var result = parser.Parse(search);

            Assert.Equal(normalized, result.ToNormalizedString());
        }

        [Theory]
        [InlineData("title:(bill)", "title:(bill)")]
        [InlineData("title:(bill AND steve) OR Paul", "title:((bill AND steve) OR Paul)")]
        [InlineData("title:((bill AND steve) OR Paul)", "title:((bill AND steve) OR Paul)")]
        public void ShouldGroup(string search, string normalized)
        {
            var parser = QueryParser(
                NamedTermParser("title",
                    ManyConditionParser<Article>(
                        ArticleManyMatch(),
                        ArticleManyNotMatch()
                    )
                )
            );

            var result = parser.Parse(search);

            Assert.Equal(search, result.ToString());
            Assert.Equal(normalized, result.ToNormalizedString());
        }

        [Fact]
        public void ShouldIgnoreMultipleNamedTerms()
        {
            var parser = QueryParser(
                NamedTermParser("name",
                    OneConditionParser(PersonOneConditionQuery())
                )
            );

            // By convention the last term is used when single = true;
            Assert.Equal("name:bill", parser.Parse("name:steve name:bill").ToString());
            Assert.Equal("name:bill", parser.Parse("name:steve name:bill").ToNormalizedString());
        }

        [Fact]
        public void ShouldAllowMultipleNamedTerms()
        {
            var parser = QueryParser(
                NamedTermParser("name",
                    OneConditionParser(PersonOneConditionQuery(), single: false)
                )
            );

            // By convention the last term is used when single = true;
            Assert.Equal("name:steve name:bill", parser.Parse("name:steve name:bill").ToString());
            Assert.Equal("name:steve name:bill", parser.Parse("name:steve name:bill").ToNormalizedString());
        }

        private static Func<string, IQuery<Person>, IQuery<Person>> PersonOneConditionQuery()
        {
            return (val, query) => query.With<PersonByName>(x => x.SomeName.Contains(val));
        }

        private static Func<string, IQuery<Person>, IQuery<Person>> PersonManyMatch()
            => PersonOneConditionQuery();

        private static Func<string, IQuery<Person>, IQuery<Person>> PersonManyNotMatch()
        {
            return (val, query) => query.With<PersonByName>(x => x.SomeName.IsNotIn<PersonByName>(s => s.SomeName, w => w.SomeName.Contains(val)));
        }

        private static Func<string, IQuery<Article>, IQuery<Article>> ArticleManyMatch()
        {
            return (val, query) => query.With<ArticleByPublishedDate>(x => x.Title.Contains(val));
        }

        private static Func<string, IQuery<Article>, IQuery<Article>> ArticleManyNotMatch()
        {
            return (val, query) => query.With<ArticleByPublishedDate>(x => x.Title.IsNotIn<ArticleByPublishedDate>(s => s.Title, w => w.Title.Contains(val)));
        }
    }
}
