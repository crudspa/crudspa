using System.Globalization;
using System.Text.RegularExpressions;
using HtmlEditorMarkup = Crudspa.Framework.Core.Client.Components.HtmlEditorMarkup;
using DataContainer = Crudspa.Content.Display.Shared.Contracts.Data.Container;

namespace Crudspa.Content.Design.Client.Models;

public static class SectionShapes
{
    private static readonly Regex LengthRegex = new(@"^\s*(-?(?:\d+|\d*\.\d+))\s*([a-z%]+)\s*$", RegexOptions.Compiled | RegexOptions.IgnoreCase);
    private static readonly Guid ArrowRightIconId = new("63F3F22E-C276-4373-B212-8190581359E0");

    public static List<SectionShapeChoice> All()
    {
        return
        [
            Blank(),
            SingleColumn(),
            TwoColumns(),
            CenteredStack(),
            QuoteSpotlight(),
            WideBand(),
            TwoUpCards(),
            ThreeUpCards(),
            FourUpCards(),
            ActionCards(),
            TwoUpMediaPanels(),
            ThreeUpMediaPanels(),
            TwoUpMediaCards(),
            ThreeUpMediaCards(),
            FourUpMediaCards(),
            Split(),
            SplitReverse(),
            SplitWithActions(),
            SplitWithActionsReverse(),
            HeroWithImage(),
            MainSidebarStack(),
            WideMediaNotes(),
            WideMediaSidebar(),
            AudioNotes(),
        ];
    }

    private static SectionShapeChoice Blank()
    {
        return Choice(
            Guid.Parse("f538f58c-97bd-483c-9904-5d2a9236b019"),
            "Blank",
            "Start from scratch with an empty section.",
            CreateSection());
    }

    private static SectionShapeChoice SingleColumn() => CreateSingleColumnShape();

    private static SectionShapeChoice TwoColumns() => CreateTwoColumnsShape();

    private static SectionShapeChoice CenteredStack() => CreateCenteredStackShape();

    private static SectionShapeChoice QuoteSpotlight() => CreateQuoteSpotlightShape();

    private static SectionShapeChoice WideBand() => CreateWideBandShape();

    private static SectionShapeChoice TwoUpCards() =>
        CreateTextCardsShape(
            Guid.Parse("92a255c6-b21a-4c57-8267-77d21a848db5"),
            "Two-Up Cards",
            "Two matching cards for paired ideas, summaries, or slide content.",
            2,
            "18em",
            "16em");

    private static SectionShapeChoice ThreeUpCards() =>
        CreateTextCardsShape(
            Guid.Parse("3314f072-d13a-4e4e-9f99-7f7e8cf06d1e"),
            "Three-Up Cards",
            "Three matching cards that wrap cleanly as space tightens.",
            3,
            "16em",
            "14em");

    private static SectionShapeChoice FourUpCards() =>
        CreateTextCardsShape(
            Guid.Parse("3b1aba75-533e-4698-a4d9-d3b6c9c9f0d7"),
            "Four-Up Cards",
            "Four compact cards for tighter grids or summary slides.",
            4,
            "14em",
            "12em");

    private static SectionShapeChoice ActionCards() => CreateActionCardsShape();

    private static SectionShapeChoice TwoUpMediaPanels() =>
        CreateMediaPanelsShape(
            Guid.Parse("b3dcbad6-8670-471f-83b2-4bdea5bc01b4"),
            "Two-Up Media Panels",
            "Two larger media panels for simple side-by-side visuals.",
            2,
            "18em",
            "16em");

    private static SectionShapeChoice ThreeUpMediaPanels() =>
        CreateMediaPanelsShape(
            Guid.Parse("385ac5b0-841f-4cff-97cd-7fc4730941ff"),
            "Three-Up Media Panels",
            "Three clean media panels for a simple responsive strip.",
            3,
            "14em",
            "12em");

    private static SectionShapeChoice TwoUpMediaCards() =>
        CreateMediaCardsShape(
            Guid.Parse("5afdd842-ddcd-4bc0-b9fc-8c3ab777173f"),
            "Two-Up Media Cards",
            "Two matching cards with a media area above supporting copy.",
            2,
            "18em",
            "16em");

    private static SectionShapeChoice ThreeUpMediaCards() =>
        CreateMediaCardsShape(
            Guid.Parse("b874fa25-afaa-462c-9c39-044c28eac242"),
            "Three-Up Media Cards",
            "Three matching media cards for highlights, examples, or grouped content.",
            3,
            "16em",
            "15em");

    private static SectionShapeChoice FourUpMediaCards() =>
        CreateMediaCardsShape(
            Guid.Parse("ad1b63d9-8b01-450c-b24c-7eddc6cd2101"),
            "Four-Up Media Cards",
            "Four compact media cards for denser responsive grids.",
            4,
            "14em",
            "13em");

    private static SectionShapeChoice Split() =>
        CreateSplitShape(
            Guid.Parse("88bb066a-57ba-4bf5-9796-158dc5ff5b5e"),
            "Split",
            "A clean two-column split with room for copy beside media.",
            false);

    private static SectionShapeChoice SplitReverse() =>
        CreateSplitShape(
            Guid.Parse("43ec2914-68fe-43a6-847d-75045a0cd4e6"),
            "Split Reverse",
            "The same two-column split with media first.",
            true);

    private static SectionShapeChoice SplitWithActions() =>
        CreateSplitWithActionsShape(
            Guid.Parse("82c3647c-70bb-4933-b3c3-88eab3d91944"),
            "Split + Actions",
            "A two-column split with room for copy, media, and a primary action.",
            false);

    private static SectionShapeChoice SplitWithActionsReverse() =>
        CreateSplitWithActionsShape(
            Guid.Parse("d6a24061-770c-485e-9177-e9627913e387"),
            "Split + Actions Reverse",
            "The same split with actions, with media first.",
            true);

    private static SectionShapeChoice HeroWithImage() => CreateHeroWithImageShape();

    private static SectionShapeChoice MainSidebarStack() => CreateMainSidebarStackShape();

    private static SectionShapeChoice WideMediaNotes() => CreateWideMediaNotesShape();

    private static SectionShapeChoice WideMediaSidebar() => CreateWideMediaSidebarShape();

    private static SectionShapeChoice AudioNotes() => CreateAudioNotesShape();

    private static SectionShapeChoice CreateSingleColumnShape()
    {
        var text = Text("<h2>Section Heading</h2><p>Use a simple text block when you only need a heading and supporting paragraph.</p>");
        SetFixedItem(text.Item, "36em", "18em", "1");
        text.Item.MaxWidth = "42em";
        text.Item.Width = "100%";

        var section = CreateSection(text);
        section.Container.JustifyContentId = JustifyContentIds.Center;
        section.Container.AlignItemsId = AlignItemsIds.Start;
        SetSectionPadding(section.Box, "2em");

        return Choice(
            Guid.Parse("73e74312-a6ba-43f7-89cd-223af89b1332"),
            "Single Column",
            "A single text column for headings, paragraphs, or slide notes.",
            section);
    }

    private static SectionShapeChoice CreateSplitWithActionsShape(Guid id, String name, String description, Boolean reverse)
    {
        var copy = Multimedia(
            MediaText(HeroHtml("Section Heading", "Add a short message that explains the main point."), item => SetFullWidthMultimediaItem(item.Item)),
            MediaButton("Primary Action", "primary-action", SetButtonChrome, item => SetButtonItem(item.Item, AlignSelfIds.Start)));

        SetPercentItem(copy.Item, "52%", "20em", "1");
        ApplyColumn(copy.RequireConfig<MultimediaElement>().Container, "0.75em");

        var image = Image();
        SetPercentItem(image.Item, "40%", "18em", "1");
        image.Item.AlignSelfId = AlignSelfIds.Center;
        SetMediaFrame(image.Box, "1em", "1em");

        var section = reverse ? CreateSection(image, copy) : CreateSection(copy, image);
        section.Container.AlignItemsId = AlignItemsIds.Center;
        section.Container.Gap = "1.5em";
        SetBandChrome(section.Box, padding: "2em");

        return Choice(id, name, description, section);
    }

    private static SectionShapeChoice CreateCenteredStackShape()
    {
        var items = new List<MultimediaItem>
        {
            MediaText(HeroHtml("Centered Heading", "Use a centered stack for concise copy and one or two actions.", true), item => SetFullRow(item.Item)),
            MediaButton("Primary Action", "primary-action", SetButtonChrome, item => SetButtonItem(item.Item, AlignSelfIds.Center)),
        };

        items.Add(MediaButton("Secondary Action", "secondary-action", SetButtonChrome, item => SetButtonItem(item.Item, AlignSelfIds.Center)));

        var hero = Multimedia(items);
        SetFixedItem(hero.Item, "38em", "18em", "1");
        hero.Item.MaxWidth = "38em";
        hero.Item.Width = "100%";

        ApplyRowWrap(hero.RequireConfig<MultimediaElement>().Container, "0.75em", JustifyContentIds.Center, AlignItemsIds.Center);
        SetCardChrome(hero.Box, padding: "2em");

        var section = CreateSection(hero);
        section.Container.JustifyContentId = JustifyContentIds.Center;
        section.Container.AlignItemsId = AlignItemsIds.Center;
        SetSectionPadding(section.Box, "2em");

        return Choice(
            Guid.Parse("92ea3fb8-91f2-4da3-8cee-7e3bc863929a"),
            "Centered Stack",
            "Centered copy with room for one or two actions.",
            section);
    }

    private static SectionShapeChoice CreateHeroWithImageShape()
    {
        var copy = Multimedia(
            MediaText(HeroHeadingHtml("Hero Heading")),
            MediaText(HeroLeadHtml(
                "Add a short lead that explains the value clearly.",
                "Use a supporting paragraph for proof, context, or the next part of the story.")),
            MediaButton("Primary Action", "primary-action", SetButtonChrome, item => SetButtonItem(item.Item, AlignSelfIds.Center)));

        SetPercentItem(copy.Item, "52%", "20em", "1");
        ApplyColumn(copy.RequireConfig<MultimediaElement>().Container, "0.75em");
        copy.Box.PaddingRight = "3em";
        copy.Box.PaddingBottom = "2em";
        copy.Box.PaddingLeft = "3em";

        var image = Image();
        SetPercentItem(image.Item, "40%", "18em", "1");
        image.Item.AlignSelfId = AlignSelfIds.Center;
        image.Box.PaddingRight = "1em";
        image.Box.PaddingLeft = "1em";

        var section = CreateSection(copy, image);
        section.Container.AlignItemsId = AlignItemsIds.Center;
        section.Container.Gap = "1.5em";
        section.Box.PaddingRight = "1em";
        section.Box.PaddingLeft = "1em";

        return Choice(
            Guid.Parse("84283e3c-0c8f-4b13-a1e0-d815fd50183f"),
            "Hero + Image",
            "A wide hero band with stacked title, supporting copy, a centered action, and an accompanying image.",
            section);
    }

    private static SectionShapeChoice CreateQuoteSpotlightShape()
    {
        var quote = Text(QuoteHtml());
        SetFixedItem(quote.Item, "40em", "20em", "1");
        quote.Item.MaxWidth = "44em";
        quote.Item.Width = "100%";
        SetCardChrome(quote.Box, shadow: false, padding: "2em");

        var section = CreateSection(quote);
        section.Container.JustifyContentId = JustifyContentIds.Center;
        section.Container.AlignItemsId = AlignItemsIds.Center;
        SetSectionPadding(section.Box, "2em");

        return Choice(
            Guid.Parse("8378779c-1185-40d1-afb2-94b8d6e34930"),
            "Quote Spotlight",
            "A centered pull quote or testimonial with room for attribution.",
            section);
    }

    private static SectionShapeChoice CreateSplitShape(Guid id, String name, String description, Boolean reverse)
    {
        var text = Text(FeatureHtml("Section Heading", "Use the second column for supporting copy, detail, or context."));
        SetPercentItem(text.Item, "50%", "18em", "1");

        var image = Image();
        SetPercentItem(image.Item, "42%", "18em", "1");
        image.Item.AlignSelfId = AlignSelfIds.Center;
        SetMediaFrame(image.Box, "1em", "1em");

        var section = reverse ? CreateSection(image, text) : CreateSection(text, image);
        section.Container.AlignItemsId = AlignItemsIds.Center;
        section.Container.Gap = "1.5em";
        SetSectionPadding(section.Box, "2em");

        return Choice(id, name, description, section);
    }

    private static SectionShapeChoice CreateMainSidebarStackShape()
    {
        var story = Text(StoryHtml());
        SetPercentItem(story.Item, "62%", "22em", "2");

        var aside = Multimedia(
            MediaText(CardHtml("Sidebar Note", "Use a short supporting point, note, or summary here."), item =>
            {
                SetFullWidthMultimediaItem(item.Item);
                SetCardChrome(item.Box, shadow: false);
            }),
            MediaText(CardHtml("Supporting Point", "Add one more short note, detail, or transition here."), item =>
            {
                SetFullWidthMultimediaItem(item.Item);
                SetCardChrome(item.Box, shadow: false);
            }));

        SetFixedItem(aside.Item, "18em", "15em", "1");
        ApplyColumn(aside.RequireConfig<MultimediaElement>().Container, "0.75em");

        var section = CreateSection(story, aside);
        section.Container.AlignItemsId = AlignItemsIds.Start;
        section.Container.Gap = "1.25em";
        SetSectionPadding(section.Box, "2em");

        return Choice(
            Guid.Parse("251dd2e8-f855-4eba-bfcf-8a5cc9c37ea4"),
            "Main + Sidebar Stack",
            "A primary column with a smaller stacked sidebar for supporting points.",
            section);
    }

    private static SectionShapeChoice CreateTextCardsShape(Guid id, String name, String description, Int32 count, String basis, String minWidth)
    {
        var cards = new List<SectionElement>();

        for (var index = 0; index < count; index++)
        {
            var card = Text(CardHtml($"Card {index + 1:D}", "Use each card for a feature, value, or short supporting message."));
            SetFixedItem(card.Item, basis, minWidth, "1");
            SetCardChrome(card.Box);
            cards.Add(card);
        }

        var section = CreateSection(cards);
        section.Container.Gap = "1em";
        SetSectionPadding(section.Box, "1em");

        return Choice(id, name, description, section);
    }

    private static SectionShapeChoice CreateMediaCardsShape(Guid id, String name, String description, Int32 count, String basis, String minWidth)
    {
        var cards = new List<SectionElement>();

        for (var index = 0; index < count; index++)
        {
            var card = Multimedia(
                MediaImage(item =>
                {
                    SetFullWidthMultimediaItem(item.Item);
                    SetMediaFrame(item.Box);
                }),
                MediaText(CardHtml($"Media Card {index + 1:D}", "Combine an image with a short headline and caption."), item => SetFullWidthMultimediaItem(item.Item)));

            SetFixedItem(card.Item, basis, minWidth, "1");
            ApplyColumn(card.RequireConfig<MultimediaElement>().Container, "0.75em");
            SetCardChrome(card.Box);
            cards.Add(card);
        }

        var section = CreateSection(cards);
        section.Container.Gap = "1em";
        SetSectionPadding(section.Box, "1em");

        return Choice(id, name, description, section);
    }

    private static SectionShapeChoice CreateTwoColumnsShape()
    {
        var left = Text("<h3>Column One</h3><p>Use the first column for supporting copy, notes, or grouped details.</p><p>Add a second paragraph when you need a little more context.</p>");
        SetFixedItem(left.Item, "22em", "18em", "1");
        SetCardChrome(left.Box);

        var right = Text("<h3>Column Two</h3><p>Use the second column for parallel copy, bullets, or follow-up detail.</p><p>Keep the two columns balanced so they stack cleanly on tall screens.</p>");
        SetFixedItem(right.Item, "22em", "18em", "1");
        SetCardChrome(right.Box);

        var section = CreateSection(left, right);
        section.Container.Gap = "1em";
        SetSectionPadding(section.Box, "1em");

        return Choice(
            Guid.Parse("0df4fddd-488b-465a-b2bd-bfceb4120e4f"),
            "Two Columns",
            "Two balanced copy columns that stack cleanly on tall screens.",
            section);
    }

    private static SectionShapeChoice CreateWideBandShape()
    {
        var cta = Multimedia(
            MediaText(BandTextHtml("Band Heading", "Use a wide band for a short summary, transition, or supporting action."), item => SetFixedItem(item.Item, "22em", "16em", "1")),
            MediaButton("Continue", "continue", SetButtonChrome, item => SetButtonItem(item.Item, AlignSelfIds.Center)));

        SetFullWidthItem(cta.Item);
        ApplyRowWrap(cta.RequireConfig<MultimediaElement>().Container, "1em", JustifyContentIds.Between, AlignItemsIds.Center);
        SetBandChrome(cta.Box, shadow: false, padding: "2em");

        var section = CreateSection(cta);
        section.Container.Gap = "0";

        return Choice(
            Guid.Parse("00941a04-fb73-485e-a66b-cc3fb6cbc278"),
            "Wide Band",
            "A full-width band with copy and one supporting action.",
            section);
    }

    private static SectionShapeChoice CreateWideMediaNotesShape()
    {
        var video = Video();
        SetPercentItem(video.Item, "54%", "18em", "1");
        SetMediaFrame(video.Box, "1em", "1em");

        var details = Multimedia(
            MediaText(FeatureHtml("Media Heading", "Use the supporting column for notes, framing, or a quick summary."), item => SetFullWidthMultimediaItem(item.Item)),
            MediaButton("Watch Next", "watch-next", SetButtonChrome, item => SetButtonItem(item.Item, AlignSelfIds.Start)));

        SetPercentItem(details.Item, "36%", "16em", "1");
        ApplyColumn(details.RequireConfig<MultimediaElement>().Container, "0.75em");
        SetCardChrome(details.Box);

        var section = CreateSection(video, details);
        section.Container.AlignItemsId = AlignItemsIds.Center;
        section.Container.Gap = "1.5em";
        SetBandChrome(section.Box, padding: "2em");

        return Choice(
            Guid.Parse("962f03ee-d2ff-457a-9384-3a1ebe4056aa"),
            "Wide Media + Notes",
            "A wide media panel beside supporting notes and one action.",
            section);
    }

    private static SectionShapeChoice CreateWideMediaSidebarShape()
    {
        var video = Video();
        SetPercentItem(video.Item, "58%", "18em", "1");
        SetMediaFrame(video.Box, "1em", "1em");

        var sidebar = Multimedia(
            MediaText(CardHtml("Key Points", "Use the sidebar for highlights or takeaways."), item => SetFullWidthMultimediaItem(item.Item)),
            MediaText(CardHtml("What To Do Next", "Add a follow-up prompt, summary, or reflection."), item => SetFullWidthMultimediaItem(item.Item)),
            MediaButton("Continue", "continue", SetButtonChrome, item => SetButtonItem(item.Item, AlignSelfIds.Start)));

        SetFixedItem(sidebar.Item, "14em", "12em", "0");
        ApplyColumn(sidebar.RequireConfig<MultimediaElement>().Container, "0.5em");
        SetCardChrome(sidebar.Box);

        var section = CreateSection(video, sidebar);
        section.Container.AlignItemsId = AlignItemsIds.Stretch;
        section.Container.Gap = "1em";
        SetSectionPadding(section.Box, "2em");

        return Choice(
            Guid.Parse("b0971026-565b-43ac-b324-f35e58565e00"),
            "Wide Media + Sidebar",
            "A wide media panel with a compact stacked sidebar.",
            section);
    }

    private static SectionShapeChoice CreateMediaPanelsShape(Guid id, String name, String description, Int32 count, String basis, String minWidth)
    {
        var images = new List<SectionElement>();

        for (var index = 0; index < count; index++)
        {
            var image = Image();
            SetFixedItem(image.Item, basis, minWidth, "1");
            SetMediaFrame(image.Box);
            images.Add(image);
        }

        var section = CreateSection(images);
        section.Container.Gap = "1em";
        SetSectionPadding(section.Box, "1em");

        return Choice(id, name, description, section);
    }

    private static SectionShapeChoice CreateActionCardsShape()
    {
        var cards = new List<SectionElement>();

        for (var index = 0; index < 3; index++)
        {
            var card = Multimedia(
                MediaText(CardHtml($"Action Card {index + 1:D}", "Use this card for a short description and one clear action."), item => SetFullWidthMultimediaItem(item.Item)),
                MediaButton("Open", $"action-{index + 1:D}", SetButtonChrome, item => SetButtonItem(item.Item, AlignSelfIds.Start)));

            SetFixedItem(card.Item, "16em", "15em", "1");
            ApplyColumn(card.RequireConfig<MultimediaElement>().Container, "0.75em");
            SetCardChrome(card.Box);
            cards.Add(card);
        }

        var section = CreateSection(cards);
        section.Container.Gap = "1em";
        SetSectionPadding(section.Box, "1em");

        return Choice(
            Guid.Parse("6929d004-826a-457a-8cf7-d04e99ed5967"),
            "Action Cards",
            "Three matching cards with room for copy and a button.",
            section);
    }

    private static SectionShapeChoice CreateAudioNotesShape()
    {
        var audio = Audio();
        SetFixedItem(audio.Item, "10em", "9em", "0");
        SetCardChrome(audio.Box, shadow: false);

        var details = Multimedia(
            MediaText(FeatureHtml("Audio Heading", "Pair audio with guidance, prompts, or supporting notes."), item => SetFullWidthMultimediaItem(item.Item)),
            MediaButton("Primary Action", "primary-action", SetButtonChrome, item => SetButtonItem(item.Item, AlignSelfIds.Start)));

        SetPercentItem(details.Item, "52%", "18em", "1");
        ApplyColumn(details.RequireConfig<MultimediaElement>().Container, "0.75em");

        var section = CreateSection(audio, details);
        section.Container.AlignItemsId = AlignItemsIds.Center;
        section.Container.Gap = "1.5em";
        SetBandChrome(section.Box, padding: "2em");

        return Choice(
            Guid.Parse("37f03fe1-fe17-440c-9510-86325e2dd28e"),
            "Audio + Notes",
            "An audio player beside supporting notes and one action.",
            section);
    }

    private static Section CreateSection(params SectionElement[] elements) => CreateSection((IEnumerable<SectionElement>)elements);

    private static Section CreateSection(IEnumerable<SectionElement> elements)
    {
        var section = new Section
        {
            TypeId = SectionTypeIds.Responsive,
            Box = new(),
            Container = new()
            {
                DirectionId = DirectionIds.Row,
                WrapId = WrapIds.Wrap,
                JustifyContentId = JustifyContentIds.Center,
                AlignItemsId = AlignItemsIds.Stretch,
                AlignContentId = AlignContentIds.Start,
                Gap = "1.25em",
            },
            Elements = elements.ToObservable(),
        };

        section.Elements.EnsureOrder();

        return section;
    }

    private static SectionShapeChoice Choice(Guid id, String name, String description, Section section)
    {
        var choice = new SectionShapeChoice
        {
            Id = id,
            Name = name,
            Description = description,
            Section = section,
        };

        choice.VariantBoxes.AddRange(CreateVariantBoxes(section));

        return choice;
    }

    private static List<SectionShapeVariantTarget> CreateVariantBoxes(Section section)
    {
        var variantBoxes = new List<SectionShapeVariantTarget>();

        TryAddVariantBox(
            variantBoxes,
            new()
            {
                Scope = SectionShapeVariantScopes.Section,
            },
            section.Box);

        foreach (var element in section.Elements.OrderBy(x => x.Ordinal))
        {
            TryAddVariantBox(
                variantBoxes,
                new()
                {
                    Scope = SectionShapeVariantScopes.Element,
                    ElementOrdinal = element.Ordinal,
                },
                element.Box);

            if (element.As<MultimediaElement>() is not { } multimediaElement)
                continue;

            foreach (var item in multimediaElement.MultimediaItems.OrderBy(x => x.Ordinal))
            {
                TryAddVariantBox(
                    variantBoxes,
                    new()
                    {
                        Scope = SectionShapeVariantScopes.MultimediaItem,
                        ElementOrdinal = element.Ordinal,
                        MultimediaItemOrdinal = item.Ordinal,
                    },
                    item.Box);
            }
        }

        return variantBoxes;
    }

    private static void TryAddVariantBox(List<SectionShapeVariantTarget> variantBoxes, SectionShapeVariantTarget variantBox, Box box)
    {
        if (!SupportsVariants(box))
            return;

        variantBox.Border = new()
        {
            Thickness = box.BorderThickness ?? "1px",
            ThicknessTop = box.BorderThicknessTop,
            ThicknessLeft = box.BorderThicknessLeft,
            ThicknessRight = box.BorderThicknessRight,
            ThicknessBottom = box.BorderThicknessBottom,
        };
        variantBox.RoundedBorderRadius = GetRoundedBorderRadius(box.BorderRadius);
        variantBox.Shadow = new()
        {
            BlurRadius = box.ShadowBlurRadius ?? "1.5em",
            Color = box.ShadowColor ?? "#00000040",
            OffsetX = box.ShadowOffsetX ?? ".6em",
            OffsetY = box.ShadowOffsetY ?? ".6em",
            SpreadRadius = box.ShadowSpreadRadius ?? ".15em",
        };

        variantBoxes.Add(variantBox);
        ClearVariantChrome(box);
    }

    private static Boolean SupportsVariants(Box box)
    {
        return box.BorderColor.HasSomething()
            || box.BorderThickness.HasSomething()
            || box.BorderThicknessTop.HasSomething()
            || box.BorderThicknessLeft.HasSomething()
            || box.BorderThicknessRight.HasSomething()
            || box.BorderThicknessBottom.HasSomething()
            || box.BorderRadius.HasSomething()
            || box.ShadowBlurRadius.HasSomething()
            || box.ShadowColor.HasSomething()
            || box.ShadowOffsetX.HasSomething()
            || box.ShadowOffsetY.HasSomething()
            || box.ShadowSpreadRadius.HasSomething();
    }

    private static void ClearVariantChrome(Box box)
    {
        box.BorderColor = null;
        box.BorderThickness = null;
        box.BorderThicknessTop = null;
        box.BorderThicknessLeft = null;
        box.BorderThicknessRight = null;
        box.BorderThicknessBottom = null;
        box.BorderRadius = null;
        ClearShadow(box);
    }

    private static String GetRoundedBorderRadius(String? borderRadius)
    {
        if (borderRadius.HasNothing())
            return "1em";

        var match = LengthRegex.Match(borderRadius!);

        if (!match.Success)
            return borderRadius!;

        var amount = Decimal.Parse(match.Groups[1].Value, CultureInfo.InvariantCulture);
        var unit = match.Groups[2].Value;
        var minimum = unit.IsBasically("px") ? 16m : 1m;
        var adjusted = Math.Max(amount * 1.5m, minimum);

        return $"{adjusted.ToString("0.###", CultureInfo.InvariantCulture)}{unit}";
    }

    private static SectionElement Text(String html)
    {
        var element = CreateElement(ElementTypeIds.TextElement);
        element.SetConfig(new TextElement { Text = HtmlEditorMarkup.NormalizeForStorage(html) });
        return element;
    }

    private static SectionElement Image()
    {
        var element = CreateElement(ElementTypeIds.Image);
        element.SetConfig(new ImageElement());
        return element;
    }

    private static SectionElement Video()
    {
        var element = CreateElement(ElementTypeIds.Video);
        element.SetConfig(new VideoElement());
        return element;
    }

    private static SectionElement Audio()
    {
        var element = CreateElement(ElementTypeIds.Audio);
        element.SetConfig(new AudioElement());
        return element;
    }

    private static SectionElement Multimedia(params MultimediaItem[] items) => Multimedia((IEnumerable<MultimediaItem>)items);

    private static SectionElement Multimedia(IEnumerable<MultimediaItem> items)
    {
        var multimediaItems = items.ToObservable();
        multimediaItems.EnsureOrder();

        var element = CreateElement(ElementTypeIds.Multimedia);
        element.SetConfig(new MultimediaElement
        {
            Container = new()
            {
                DirectionId = DirectionIds.Row,
                WrapId = WrapIds.Wrap,
                JustifyContentId = JustifyContentIds.Center,
                AlignItemsId = AlignItemsIds.Stretch,
                AlignContentId = AlignContentIds.Start,
                Gap = "0.75em",
            },
            MultimediaItems = multimediaItems,
        });

        return element;
    }

    private static SectionElement CreateElement(Guid typeId)
    {
        return new()
        {
            Element = new()
            {
                TypeId = typeId,
                Box = new(),
                Item = new()
                {
                    BasisId = BasisIds.Auto,
                    Grow = "0",
                    Shrink = "1",
                    AlignSelfId = AlignSelfIds.Auto,
                },
            },
        };
    }

    private static MultimediaItem MediaText(String html, Action<MultimediaItem>? configure = null)
    {
        var item = CreateMultimediaItem(MultimediaItem.MediaTypes.Text);
        item.Text = HtmlEditorMarkup.NormalizeForStorage(html);
        configure?.Invoke(item);
        return item;
    }

    private static MultimediaItem MediaImage(Action<MultimediaItem>? configure = null)
    {
        var item = CreateMultimediaItem(MultimediaItem.MediaTypes.Image);
        configure?.Invoke(item);
        return item;
    }

    private static MultimediaItem MediaButton(String text, String path, Action<Button>? configureButton = null, Action<MultimediaItem>? configureItem = null)
    {
        var item = CreateMultimediaItem(MultimediaItem.MediaTypes.Button);
        item.Button = new()
        {
            Internal = true,
            Path = path,
            Text = text,
            ShapeIndex = Button.Shapes.Rectangle,
            GraphicIndex = Button.Graphics.Icon,
            TextTypeIndex = Button.TextTypes.Custom,
            OrientationIndex = Button.Orientations.Right,
            IconId = ArrowRightIconId,
            Box = new(),
            IconCssClass = "c-icon-arrow-right",
        };

        configureButton?.Invoke(item.Button);
        configureItem?.Invoke(item);

        return item;
    }

    private static MultimediaItem CreateMultimediaItem(MultimediaItem.MediaTypes mediaTypeIndex)
    {
        return new()
        {
            Id = Guid.NewGuid(),
            MediaTypeIndex = mediaTypeIndex,
            Box = new(),
            Item = new()
            {
                BasisId = BasisIds.Auto,
                Grow = "0",
                Shrink = "1",
                AlignSelfId = AlignSelfIds.Auto,
            },
        };
    }

    private static void ApplyRowWrap(DataContainer container, String gap, Guid? justifyContentId, Guid? alignItemsId)
    {
        container.DirectionId = DirectionIds.Row;
        container.WrapId = WrapIds.Wrap;
        container.JustifyContentId = justifyContentId;
        container.AlignItemsId = alignItemsId;
        container.AlignContentId = AlignContentIds.Start;
        container.Gap = gap;
    }

    private static void ApplyColumn(DataContainer container, String gap)
    {
        container.DirectionId = DirectionIds.Column;
        container.WrapId = WrapIds.None;
        container.JustifyContentId = JustifyContentIds.Start;
        container.AlignItemsId = AlignItemsIds.Stretch;
        container.AlignContentId = AlignContentIds.Start;
        container.Gap = gap;
    }

    private static void SetPercentItem(Item item, String basisAmount, String minWidth, String grow)
    {
        item.BasisId = BasisIds.Percentage;
        item.BasisAmount = basisAmount;
        item.Grow = grow;
        item.Shrink = "1";
        item.AlignSelfId = AlignSelfIds.Stretch;
        item.MinWidth = minWidth;
        item.MaxWidth = null;
        item.Width = null;
    }

    private static void SetFixedItem(Item item, String basisAmount, String minWidth, String grow)
    {
        item.BasisId = BasisIds.Fixed;
        item.BasisAmount = basisAmount;
        item.Grow = grow;
        item.Shrink = "1";
        item.AlignSelfId = AlignSelfIds.Stretch;
        item.MinWidth = minWidth;
        item.MaxWidth = null;
        item.Width = null;
    }

    private static void SetFullRow(Item item)
    {
        item.BasisId = BasisIds.Percentage;
        item.BasisAmount = "100%";
        item.Grow = "1";
        item.Shrink = "1";
        item.AlignSelfId = AlignSelfIds.Stretch;
        item.MinWidth = null;
        item.MaxWidth = null;
        item.Width = "100%";
    }

    private static void SetFullWidthMultimediaItem(Item item)
    {
        item.BasisId = BasisIds.Auto;
        item.BasisAmount = null;
        item.Grow = "0";
        item.Shrink = "1";
        item.AlignSelfId = AlignSelfIds.Stretch;
        item.MinWidth = null;
        item.MaxWidth = null;
        item.Width = "100%";
    }

    private static void SetFullWidthItem(Item item)
    {
        item.BasisId = BasisIds.Percentage;
        item.BasisAmount = "100%";
        item.Grow = "1";
        item.Shrink = "1";
        item.AlignSelfId = AlignSelfIds.Stretch;
        item.MinWidth = "18em";
        item.MaxWidth = null;
        item.Width = "100%";
    }

    private static void SetButtonItem(Item item, Guid? alignSelfId)
    {
        item.BasisId = BasisIds.Auto;
        item.BasisAmount = null;
        item.Grow = "0";
        item.Shrink = "0";
        item.AlignSelfId = alignSelfId ?? AlignSelfIds.Auto;
        item.MinWidth = null;
        item.MaxWidth = null;
        item.Width = null;
    }

    private static void SetBandChrome(Box box, String padding = "1em", String radius = ".5em", Boolean shadow = true)
    {
        box.BorderThickness = "1px";
        box.BorderRadius = radius;
        SetPadding(box, padding);
        if (shadow)
            SetShadow(box);
        else
            ClearShadow(box);
    }

    private static void SetCardChrome(Box box, Boolean shadow = true, String padding = "1em", String radius = ".5em")
    {
        box.BorderThickness = "1px";
        box.BorderRadius = radius;
        SetPadding(box, padding);

        if (shadow)
            SetShadow(box);
        else
            ClearShadow(box);
    }

    private static void SetMediaFrame(Box box, String padding = "1em", String radius = ".5em")
    {
        box.BorderThickness = "1px";
        box.BorderRadius = radius;
        SetPadding(box, padding);
        ClearShadow(box);
    }

    private static void SetButtonChrome(Button button)
    {
        button.ShapeIndex = Button.Shapes.Rectangle;
        button.GraphicIndex = Button.Graphics.Icon;
        button.TextTypeIndex = Button.TextTypes.Custom;
        button.OrientationIndex = Button.Orientations.Right;
        button.IconId = ArrowRightIconId;
        button.IconCssClass = "c-icon-arrow-right";
    }

    private static void SetShadow(Box box, String offsetX = ".6em", String offsetY = ".6em", String blur = "1.5em", String spread = ".15em")
    {
        box.ShadowColor = "#00000040";
        box.ShadowOffsetX = offsetX;
        box.ShadowOffsetY = offsetY;
        box.ShadowBlurRadius = blur;
        box.ShadowSpreadRadius = spread;
    }

    private static void ClearShadow(Box box)
    {
        box.ShadowColor = null;
        box.ShadowOffsetX = null;
        box.ShadowOffsetY = null;
        box.ShadowBlurRadius = null;
        box.ShadowSpreadRadius = null;
    }

    private static void SetSectionPadding(Box box, String all) => SetPadding(box, all);

    private static void SetPadding(Box box, String all) => SetPadding(box, all, all);

    private static void SetPadding(Box box, String vertical, String horizontal)
    {
        box.PaddingTop = vertical;
        box.PaddingBottom = vertical;
        box.PaddingLeft = horizontal;
        box.PaddingRight = horizontal;
    }

    private static String HeroHtml(String heading, String body, Boolean centered = false)
    {
        return centered
            ? $"<h2 style=\"text-align: center;\">{heading}</h2><p style=\"text-align: center;\">{body}</p>"
            : $"<h2>{heading}</h2><p>{body}</p>";
    }

    private static String HeroHeadingHtml(String heading)
    {
        return $"<h1>{heading}</h1>";
    }

    private static String HeroLeadHtml(String lead, String body)
    {
        return $"<h4>{lead}</h4><p>{body}</p>";
    }

    private static String FeatureHtml(String heading, String body)
    {
        return $"<h2>{heading}</h2><p>{body}</p><ul><li>Key point one</li><li>Key point two</li><li>Key point three</li></ul>";
    }

    private static String StoryHtml()
    {
        return "<h2>Main Column Heading</h2><p>Use the main column for longer copy, grouped notes, or a section introduction.</p><p>Add a second paragraph when you need a little more detail.</p>";
    }

    private static String CardHtml(String heading, String body)
    {
        return $"<h3>{heading}</h3><p>{body}</p>";
    }

    private static String QuoteHtml()
    {
        return "<blockquote><p>Use a pull quote or testimonial when one strong voice should carry the section.</p><p><strong>Quoted Person</strong></p></blockquote>";
    }

    private static String BandTextHtml(String heading, String body)
    {
        return $"<h2>{heading}</h2><p>{body}</p>";
    }
}