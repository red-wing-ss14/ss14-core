// SPDX-FileCopyrightText: 2022 Flipp Syder <76629141+vulppine@users.noreply.github.com>
// SPDX-FileCopyrightText: 2022 metalgearsloth <31366439+metalgearsloth@users.noreply.github.com>
// SPDX-FileCopyrightText: 2022 metalgearsloth <comedian_vs_clown@hotmail.com>
// SPDX-FileCopyrightText: 2022 wrexbe <81056464+wrexbe@users.noreply.github.com>
// SPDX-FileCopyrightText: 2023 DrSmugleaf <DrSmugleaf@users.noreply.github.com>
// SPDX-FileCopyrightText: 2023 Leon Friedrich <60421075+ElectroJr@users.noreply.github.com>
// SPDX-FileCopyrightText: 2023 Visne <39844191+Visne@users.noreply.github.com>
// SPDX-FileCopyrightText: 2023 csqrb <56765288+CaptainSqrBeard@users.noreply.github.com>
// SPDX-FileCopyrightText: 2024 Piras314 <p1r4s@proton.me>
// SPDX-FileCopyrightText: 2025 Aiden <28298836+Aidenkrz@users.noreply.github.com>
//
// SPDX-License-Identifier: AGPL-3.0-or-later

using System.Linq;
using Robust.Shared.Serialization;

namespace Content.Shared.Humanoid.Markings
{
    [DataDefinition]
    [Serializable, NetSerializable]
    public sealed partial class Marking : IEquatable<Marking>, IComparable<Marking>, IComparable<string>
    {
        [DataField("markingColor")]
        private List<Color> _markingColors = new();

        // Amour edit start: secondary gradient colors per sprite layer
        [DataField("markingSecondaryColor")]
        private List<Color>? _secondaryColors;
        // Amour edit end
        private Marking()
        {
        }

        public Marking(string markingId,
            List<Color> markingColors)
        {
            MarkingId = markingId;
            _markingColors = markingColors;
        }

        public Marking(string markingId,
            IReadOnlyList<Color> markingColors)
            : this(markingId, new List<Color>(markingColors))
        {
        }

        public Marking(string markingId, int colorCount)
        {
            MarkingId = markingId;
            List<Color> colors = new();
            for (int i = 0; i < colorCount; i++)
                colors.Add(Color.White);
            _markingColors = colors;
        }

        public Marking(Marking other)
        {
            MarkingId = other.MarkingId;
            _markingColors = new(other.MarkingColors);
            Visible = other.Visible;
            Forced = other.Forced;
            // Amour edit start
            UseGradient = other.UseGradient;
            if (other._secondaryColors != null)
                _secondaryColors = new List<Color>(other._secondaryColors);
            // Amour edit end
        }

        /// <summary>
        ///     ID of the marking prototype.
        /// </summary>
        [DataField("markingId", required: true)]
        public string MarkingId { get; private set; } = default!;

        /// <summary>
        ///     All colors currently on this marking.
        /// </summary>
        [ViewVariables]
        public IReadOnlyList<Color> MarkingColors => _markingColors;

        // Amour edit start
        /// <summary>
        ///     Secondary colors used as the second stop of a vertical gradient
        ///     when <see cref="UseGradient"/> is enabled. May be null if the
        ///     marking has never been switched into gradient mode.
        /// </summary>
        [ViewVariables]
        public IReadOnlyList<Color>? SecondaryMarkingColors => _secondaryColors;
        // Amour edit end
        /// <summary>
        ///     If this marking is currently visible.
        /// </summary>
        [DataField("visible")]
        public bool Visible = true;

        /// <summary>
        ///     If this marking should be forcefully applied, regardless of points.
        /// </summary>
        [ViewVariables]
        public bool Forced;

        // Amour edit start: gradient coloring for markings (hair/slime body etc.)
        /// <summary>
        ///     If true, the marking is rendered using a vertical gradient between
        ///     the first and the second color from <see cref="MarkingColors"/>.
        ///     Applied client-side via a dedicated shader.
        /// </summary>
        [DataField("useGradient")]
        public bool UseGradient;
        // Amour edit end
        public void SetColor(int colorIndex, Color color) =>
            _markingColors[colorIndex] = color;

        public void SetColor(Color color)
        {
            for (int i = 0; i < _markingColors.Count; i++)
            {
                _markingColors[i] = color;
            }
        }

        // Amour edit start: helpers for gradient access
        /// <summary>
        ///     Returns the secondary gradient color for the given layer index,
        ///     or the primary color if no secondary is set / gradient is disabled.
        /// </summary>
        public Color GetGradientColor(int colorIndex)
        {
            if (_secondaryColors != null && colorIndex >= 0 && colorIndex < _secondaryColors.Count)
                return _secondaryColors[colorIndex];
            if (colorIndex >= 0 && colorIndex < _markingColors.Count)
                return _markingColors[colorIndex];
            return Color.White;
        }

        public void SetGradientColor(int colorIndex, Color color)
        {
            _secondaryColors ??= new List<Color>();
            while (_secondaryColors.Count <= colorIndex)
                _secondaryColors.Add(colorIndex < _markingColors.Count ? _markingColors[colorIndex] : Color.White);
            _secondaryColors[colorIndex] = color;
        }
        // Amour edit end

        public int CompareTo(Marking? marking)
        {
            if (marking == null)
            {
                return 1;
            }

            return string.Compare(MarkingId, marking.MarkingId, StringComparison.Ordinal);
        }

        public int CompareTo(string? markingId)
        {
            if (markingId == null)
                return 1;

            return string.Compare(MarkingId, markingId, StringComparison.Ordinal);
        }

        public bool Equals(Marking? other)
        {
            if (other == null)
            {
                return false;
            }
            return MarkingId.Equals(other.MarkingId)
                && _markingColors.SequenceEqual(other._markingColors)
                && Visible.Equals(other.Visible)
                && Forced.Equals(other.Forced)
                // Amour edit start
                && UseGradient.Equals(other.UseGradient)
                && SecondaryColorsEqual(_secondaryColors, other._secondaryColors);
                // Amour edit end
        }

        // Amour edit start
        private static bool SecondaryColorsEqual(List<Color>? a, List<Color>? b)
        {
            if (a == null && b == null)
                return true;
            if (a == null || b == null)
                return false;
            return a.SequenceEqual(b);
        }
        // Amour edit end
        // VERY BIG TODO: TURN THIS INTO JSONSERIALIZER IMPLEMENTATION


        // look this could be better but I don't think serializing
        // colors is the correct thing to do
        //
        // this is still janky imo but serializing a color and feeding
        // it into the default JSON serializer (which is just *fine*)
        // doesn't seem to have compatible interfaces? this 'works'
        // for now but should eventually be improved so that this can,
        // in fact just be serialized through a convenient interface
        new public string ToString()
        {
            // reserved character
            string sanitizedName = this.MarkingId.Replace('@', '_');
            List<string> colorStringList = new();
            foreach (Color color in _markingColors)
                colorStringList.Add(color.ToHex());

            // Amour edit start: append optional gradient sections
            // Format: id@col1,col2,...[|sec1,sec2,...][!G]
            var result = $"{sanitizedName}@{String.Join(',', colorStringList)}";
            if (_secondaryColors != null && _secondaryColors.Count > 0)
            {
                List<string> secondaryStringList = new();
                foreach (Color color in _secondaryColors)
                    secondaryStringList.Add(color.ToHex());
                result += $"|{String.Join(',', secondaryStringList)}";
            }
            if (UseGradient)
                result += "!G";
            return result;
            // Amour edit end
        }

        public static Marking? ParseFromDbString(string input)
        {
            if (input.Length == 0) return null;
            var split = input.Split('@');
            if (split.Length != 2) return null;

            // Amour edit start: parse optional gradient sections
            var payload = split[1];
            var useGradient = false;
            if (payload.EndsWith("!G", StringComparison.Ordinal))
            {
                useGradient = true;
                payload = payload.Substring(0, payload.Length - 2);
            }

            List<Color>? secondaryList = null;
            var pipeIdx = payload.IndexOf('|');
            if (pipeIdx >= 0)
            {
                var secPart = payload.Substring(pipeIdx + 1);
                payload = payload.Substring(0, pipeIdx);
                if (secPart.Length > 0)
                {
                    secondaryList = new List<Color>();
                    foreach (string color in secPart.Split(','))
                        secondaryList.Add(Color.FromHex(color));
                }
            }

            List<Color> colorList = new();
            foreach (string color in payload.Split(','))
                colorList.Add(Color.FromHex(color));

            var marking = new Marking(split[0], colorList)
            {
                UseGradient = useGradient,
            };
            if (secondaryList != null)
                marking._secondaryColors = secondaryList;
            return marking;
            // Amour edit end
        }
    }
}
