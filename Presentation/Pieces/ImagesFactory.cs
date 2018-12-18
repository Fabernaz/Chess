using System;
using System.Collections.Generic;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media.Imaging;
using ChessCore;

namespace Presentation
{
    public interface IImagesFactory
    {
        BitmapImage GetPieceImage(Piece piece);

        Cursor GetPieceCursor(Piece piece, double size);
    }

    public class ImagesFactory : IImagesFactory
    {
        private const string BASE_URI = "pack://application:,,,/Pieces/Images/";
        private const string KING = "King";
        private const string BISHOP = "Bishop";
        private const string KNIGHT = "Knight";
        private const string QUEEN = "Queen";
        private const string ROOK = "Rook";
        private const string PAWN = "Pawn";
        private const string WHITE = "White";
        private const string BLACK = "Black";

        private readonly IDictionary<string, Uri> _urisDict;
        private readonly IDictionary<string, BitmapImage> _imagesDict;
        private readonly IDictionary<string, Image> _cursorDict;

        public ImagesFactory()
        {
            _urisDict = new Dictionary<string, Uri>();
            _imagesDict = new Dictionary<string, BitmapImage>();
            _cursorDict = new Dictionary<string, Image>();
            ResetImagesDict();
        }

        private void ResetImagesDict()
        {
            foreach (var pieceName in GetAllPiecesNames())
            {
                var uri = new Uri(BASE_URI + pieceName + ".png");
                var bitmapImage = new BitmapImage(uri);
                var image = new Image { Source = bitmapImage };

                _cursorDict.Add(pieceName, image);
                _imagesDict.Add(pieceName, bitmapImage);
            }
        }

        public BitmapImage GetPieceImage(Piece piece)
        {
            if (piece == null)
                return null;

            return _imagesDict[GetPieceName(piece)];
        }

        public Cursor GetPieceCursor(Piece piece, double size)
        {
            var image = new Image
            {
                Source = _imagesDict[GetPieceName(piece)],
                Height = size,
                Width = size
            };

            return CursorHelper.CreateCursor(image);
        }

        #region Naming

        private string GetPieceName(Piece piece)
        {
            if (piece == null)
                return null;

            var name = piece.Color.IsWhite ? WHITE : BLACK;

            if (piece is King)
                return name + KING;
            if (piece is Bishop)
                return name + BISHOP;
            if (piece is Pawn)
                return name + PAWN;
            if (piece is Queen)
                return name + QUEEN;
            if (piece is Knight)
                return name + KNIGHT;
            if (piece is Rook)
                return name + ROOK;
            else
                throw new NotImplementedException();
        }

        private IEnumerable<string> GetAllPiecesNames()
        {
            yield return WHITE + BISHOP;
            yield return BLACK + BISHOP;

            yield return WHITE + QUEEN;
            yield return BLACK + QUEEN;

            yield return WHITE + ROOK;
            yield return BLACK + ROOK;

            yield return WHITE + KING;
            yield return BLACK + KING;

            yield return WHITE + PAWN;
            yield return BLACK + PAWN;

            yield return WHITE + KNIGHT;
            yield return BLACK + KNIGHT;
        }

        #endregion
    }
}
