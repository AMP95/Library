using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.ComponentModel;
using Library.Model;
using Library.View;
using System.Net.WebSockets;

namespace Library.ViewModel
{
    class MainViewModel : INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler? PropertyChanged;
        /// <summary>
        /// Вспомогательный метод для активации события PropertyChanged
        /// </summary>
        /// <param name="name">Название изменившегося поля</param>
        void Notify(string name) => PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(name));
        IDataManager<Book> _model;
        bool _isUnsavedChages = false;  //несохраненные в дб изменения
        List<Book> _viewedBooks;        //Отображаемая колекция
        Book _editedBook;               //Редактируемая книга
        Book _selectedBook;             //Выбранная книга
        List<Book> _selectedBooks;      //Выделенные на удаление книги
        object _mainControl;            //Главный контрол с коллекцией
        object _addEditControl;         //Контрол добавления/редактирования
        object _currentControl;         //Текущий контрол
        string _searchRes;              //Выводимая строка информирующая о текущщем поиске
        string _search;                 //Вводныя строка для поиска
        public object CurrentControl
        {
            get { return _currentControl; }
            set {
                _currentControl = value;
                Notify("CurrentControl");
            }
        }
        public string SearchRes
        {
            get { return _searchRes; }
            set {
                _searchRes = value;
                Notify("SearchRes");
            }
        }
        public string Search
        {
            get { return _search; }
            set
            {
                _search = value;
                Notify("Search");
            }
        }
        public Book EditedBook
        {
            get { return _editedBook; }
            set
            {
                _editedBook = value;
                Notify("EditedBook");
            }
        }
        public List<Book> ViewedBooks
        {
            get { return _viewedBooks; }
            set
            {
                _viewedBooks = value;
                Notify("ViewedBooks");
            }
        }
        public List<Book> SelectedBooks
        {
            get { return _selectedBooks; }
            set
            {
                _selectedBooks = value;
                Notify("SelectedBooks");
            }
        }
        public List<string> BookThemes { get; set; }
        public MainViewModel() {
            _model = new EFBookModel();
            _mainControl = new MainControl();
            _addEditControl = new AddEditControl();
            ViewedBooks = _model.GetCollection().ToList();
            EditedBook = new Book();
            SelectedBooks = new List<Book>();
            BookThemes = Enum.GetNames<BookTheme>().ToList();
            CurrentControl = _mainControl;
        }
        /// <summary>
        /// Открытие Add/Edit формы на добавление
        /// </summary>
        public ButtonCommand AddItemsCommand {
            get {
                return new ButtonCommand(
                    (param) => { CurrentControl = _addEditControl; }
                    );
            }
        }
        /// <summary>
        /// Старт поиска
        /// </summary>
        public ButtonCommand SearchCommand {
            get {
                return new ButtonCommand(
                    (param) => {
                        if (param is string text) {
                            ViewedBooks = new List<Book>((from b in ViewedBooks
                                                          where b.Name.ToLower().Contains(text.ToLower())
                                                          select b).ToList());
                            // Поисковая строкаотчщается, ее содержимое
                            // отображается в строке результат поиска
                            SearchRes = text;
                            Search = "";
                        }
                    },
                    (param) => { return param != null && param.ToString() != ""; }
                    );
            }
        }
        /// <summary>
        /// Сброс поиска
        /// </summary>
        public ButtonCommand ClearSearchCommand
        {
            get {
                return new ButtonCommand(
                    (param) => { 
                        ViewedBooks = new List<Book>(_model.GetCollection());
                        SearchRes = null;
                        Search = "";
                    }
                    );
            }
        }
        /// <summary>
        /// Открытие Add/Edit формы на редактирование
        /// </summary>
        public ButtonCommand EditCommand
        {
            get {
                return new ButtonCommand(
                        (param) => {
                                if (param is int id) {
                                _selectedBook = ViewedBooks.Find(b=> b.Id == id);
                                EditedBook = new Book()
                                {
                                    Id = _selectedBook.Id,
                                    Name = _selectedBook.Name,
                                    Author = _selectedBook.Author,
                                    YearOfIssue = _selectedBook.YearOfIssue,
                                    Theme = _selectedBook.Theme
                                };
                                CurrentControl = _addEditControl;
                            }
                        }
                    );
            }
        }
        /// <summary>
        /// Удаление единичной книги
        /// </summary>
        public ButtonCommand DeleteItemCommand
        {
            get {
                return new ButtonCommand(
                        (param) => {
                            if (param is int id) {
                                _selectedBook = ViewedBooks.Find(b => b.Id == id);
                                _model.DeleteItem(_selectedBook);
                                ViewedBooks.Remove(_selectedBook);
                                 _selectedBook = null;
                                ViewedBooks = new List<Book>(ViewedBooks);
                                _isUnsavedChages = true;
                            }
                        },
                        (param) => {
                            return !_isUnsavedChages;
                        }
                    );
            }
        }
        /// <summary>
        /// Удаление нескольких выдленных книг
        /// </summary>
        public ButtonCommand DeleteItemsCommand {
            get
            {
                return new ButtonCommand(
                        (param) => {
                            _model.DeleteItems(SelectedBooks);
                            ViewedBooks.RemoveAll((b) => {
                                return SelectedBooks.Find(bb => bb.Id == b.Id)!= null;
                            });
                            //отчистка view
                            ViewedBooks = new List<Book>(ViewedBooks);
                            SelectedBooks = new List<Book>();
                            _isUnsavedChages = true;
                        },
                        (param) => { return SelectedBooks.Count != 0 
                            || _isUnsavedChages; }
                    );
            }
        }
        /// <summary>
        /// Добавление книги, форма открыта, дальнейшее добавление
        /// </summary>
        public ButtonCommand AddCommand
        {
            get {
                return new ButtonCommand(
                        (param) => {
                            _isUnsavedChages = true;
                            ViewedBooks.Add(EditedBook);
                            _model.AddItem(EditedBook);
                            //отчистка view
                            EditedBook = new Book();
                        },
                        (param) => { return EditedBook.Name!=""; }
                    );
            }
        }
        /// <summary>
        /// Добавление для Add подтверждение для Edit, закрытие формы
        /// </summary>
        public ButtonCommand SaveAndExitCommand
        {
            get {
                return new ButtonCommand(
                    (param) => {
                        if (_selectedBook == null)
                        {
                            //Если _selectedBook пуста, значит открыто окно добавления
                            //При закрытии сохраняется добавленная книга
                            ViewedBooks.Add(EditedBook);
                            _model.AddItem(EditedBook);
                        }
                        else {
                            //открыто окно редактирования
                            //сохранение внесенных изменений
                            _model.EditItem(EditedBook);
                            int index = ViewedBooks.IndexOf(_selectedBook);
                            ViewedBooks[index] = EditedBook;
                        }
                        _isUnsavedChages = true;
                        //отчистка view
                        EditedBook = new Book();
                        ViewedBooks = new List<Book>(ViewedBooks);
                        CurrentControl = _mainControl;
                    },
                    (param) => { return EditedBook.Name != ""; }
                    );
            }
        }
        /// <summary>
        /// Отмена последнего изменения/добавление, закрытие формы
        /// </summary>
        public ButtonCommand CancelCommand
        {
            get {
                return new ButtonCommand(
                    (param) => {
                        //отчистка view
                        EditedBook = new Book();
                        CurrentControl = _mainControl;
                    }
                    );
            }
        }
        /// <summary>
        /// Сортировка коллекции
        /// </summary>
        public ButtonCommand SortCommand {
            get {
                return new ButtonCommand(
                        (param) => {
                            //каждый param соотвествует отдельному RadioButton
                            if (param is string tag) { 
                                switch(tag)
                                {
                                    case "nameA":ViewedBooks = ViewedBooks.OrderBy(b=>b.Name).ToList(); break;
                                    case "nameD":ViewedBooks = ViewedBooks.OrderByDescending(b=>b.Name).ToList(); break;
                                    case "authA":ViewedBooks = ViewedBooks.OrderBy(b=>b.Author).ToList(); break;
                                    case "authD":ViewedBooks = ViewedBooks.OrderByDescending(b=>b.Author).ToList(); break;
                                    case "yearA":ViewedBooks = ViewedBooks.OrderBy(b=>b.YearOfIssue).ToList(); break;
                                    case "yearD":ViewedBooks = ViewedBooks.OrderByDescending(b => b.YearOfIssue).ToList(); break;
                                }
                                ViewedBooks = new List<Book>(ViewedBooks);
                            }
                        }
                    );
            }
        }
        /// <summary>
        /// Сохранение всех изменений в ДБ
        /// </summary>
        public ButtonCommand SaveChangesCommand
        {
            get {
                return new ButtonCommand(
                        (param) => {
                            _model.SaveData();
                            _isUnsavedChages = false;
                        },
                        (param) => { return _isUnsavedChages; }
                    );
            }
        }
        /// <summary>
        /// Добавление книги в список SelectedBooks на удаление
        /// </summary>
        public ButtonCommand AddDeletItems
        {
            get { 
                return new ButtonCommand(
                    (param) => {
                        if (param is int id) {
                            SelectedBooks.Add(ViewedBooks.Find(b => b.Id == id));
                        }
                    }    
                ); }
        }
        /// <summary>
        /// Удаление книги из списка SelectedBooks
        /// </summary>
        public ButtonCommand DelDeletItems
        {
            get
            {
                return new ButtonCommand(
                    (param) => {
                        if (param is int id)
                        {
                            SelectedBooks.RemoveAll(b => b.Id == id);
                        }
                    }
                );
            }
        }
        /// <summary>
        /// Активация мультиселекта и отчистка SelectedBooks от прошлых выделений
        /// </summary>
        public ButtonCommand MultiselectButtonCommand
        {
            get {
                return new ButtonCommand(
                       (param)=> { 
                           SelectedBooks = new List<Book>(); 
                       } 
                    ) ; 
            }
        }
    }
}
