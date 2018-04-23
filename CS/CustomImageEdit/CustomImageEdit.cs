using System;
using System.Collections.Generic;
using System.Text;
using DevExpress.XtraEditors;
using DevExpress.XtraEditors.Repository;
using DevExpress.XtraEditors.Registrator;
using DevExpress.XtraEditors.ViewInfo;
using DevExpress.XtraEditors.Drawing;
using DevExpress.XtraEditors.Controls;
using DevExpress.XtraEditors.Popup;
using System.ComponentModel;
using System.Drawing;
using System.Windows.Forms;
using System.Reflection;

namespace MyCustomEdit
{
    [UserRepositoryItem("RegisterCustomImageEdit")]
    public class RepositoryItemCustomImageEdit : RepositoryItemPopupBase
    {
        protected int loadButtonIndex;
        OpenFileDialog openFile;
        public RepositoryItemCustomImageEdit()
            : base()
        {
            ActionButtonIndex = 1;
            loadButtonIndex = 0;
        }
        public virtual int LoadButtonIndex { get { return loadButtonIndex; } }

        public override void CreateDefaultButton()
        {
            EditorButton btn = new EditorButton();
            btn.IsDefaultButton = true;
            btn.Kind = ButtonPredefines.Plus;
            Buttons.Add(btn);
            base.CreateDefaultButton();
        }

        public virtual void LoadFile()
        {
            if (openFile == null) openFile = new OpenFileDialog();
            if (openFile.ShowDialog() == System.Windows.Forms.DialogResult.OK)
                OwnerEdit.EditValue = openFile.FileName;
        }

        static RepositoryItemCustomImageEdit() { RegisterCustomImageEdit(); }

        public const string CustomImageEditName = "CustomImageEdit";

        public override string EditorTypeName { get { return CustomImageEditName; } }


        public static void RegisterCustomImageEdit()
        {
            EditorRegistrationInfo.Default.Editors.Add(new EditorClassInfo(CustomImageEditName,
              typeof(CustomImageEdit), typeof(RepositoryItemCustomImageEdit),
              typeof(PopupBaseEditViewInfo), new ButtonEditPainter(), true));
        }


    }

    public class CustomImageEdit : PopupBaseEdit
    {
        public CustomImageEdit() : base() { }

        static CustomImageEdit() { RepositoryItemCustomImageEdit.RegisterCustomImageEdit(); }

        public override string EditorTypeName { get { return RepositoryItemCustomImageEdit.CustomImageEditName; } }
        
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Content)]
        public new RepositoryItemCustomImageEdit Properties
        {
            get { return base.Properties as RepositoryItemCustomImageEdit; }
        }

        protected override DevExpress.XtraEditors.Popup.PopupBaseForm CreatePopupForm()
        {
            return new CustomPopupBaseSizeableForm(this);
        }

        protected override void OnPressButton(EditorButtonObjectInfoArgs buttonInfo)
        {
            if (IsLoadButton(buttonInfo)) Properties.LoadFile();
            base.OnPressButton(buttonInfo);
        }

        protected virtual bool IsLoadButton(EditorButtonObjectInfoArgs buttonInfo)
        {
            return Properties.LoadButtonIndex >= 0 && Properties.Buttons.IndexOf(buttonInfo.Button) == Properties.LoadButtonIndex;
        }
    }

    public class CustomPopupBaseSizeableForm : PopupBaseSizeableForm
    {
        public Image myImage;
        SimpleButton openButton, okButton, cancelButton, clearButton;
        OpenFileDialog openFile;
        string resultFileName;
        Size formContentSize;

        public CustomPopupBaseSizeableForm(PopupBaseEdit ownerEdit)
            : base(ownerEdit)
        {
            CreateButtons();
            openFile = new OpenFileDialog();
            formContentSize = new Size(200, 200);
        }

        protected virtual void CreateButtons()
        {
            okButton = new SimpleButton();
            okButton.Text = "Ok";
            okButton.AllowFocus = false;
            okButton.Click += new EventHandler(okButton_Click);
            Controls.Add(okButton);

            cancelButton = new SimpleButton();
            cancelButton.Text = "Cancel";
            cancelButton.AllowFocus = false;
            cancelButton.Click += new EventHandler(cancelButton_Click);
            Controls.Add(cancelButton);

            openButton = new SimpleButton();
            openButton.Text = "Open";
            openButton.AllowFocus = false;
            openButton.Click += new EventHandler(openButton_Click);
            Controls.Add(openButton);

            clearButton = new SimpleButton();
            clearButton.Text = "Clear";
            clearButton.AllowFocus = false;
            clearButton.Click += new EventHandler(clearButton_Click);
            Controls.Add(clearButton);
        }
        protected override Size MinFormSize
        {
            get
            {
                Size newSize = base.MinFormSize;
                newSize.Width = ViewInfo.ButtonSize.Width * 5;
                return newSize;
            }
        }
        protected virtual void ChangeButtonsPositions()
        {
            okButton.Bounds = ViewInfo.OkButtonRect;
            cancelButton.Bounds = ViewInfo.CancelButtonRect;
            openButton.Bounds = ViewInfo.OpenButtonRect;
            clearButton.Bounds = ViewInfo.ClearButtonRect;
        }

        void okButton_Click(object sender, EventArgs e)
        {
            ClosePopup(PopupCloseMode.Normal);
        }

        void cancelButton_Click(object sender, EventArgs e)
        {
            ClosePopup(PopupCloseMode.Cancel);
        }

        void clearButton_Click(object sender, EventArgs e)
        {
            myImage = null;
            openFile.FileName = "";
            resultFileName = "";
            LayoutChanged();
        }

        void openButton_Click(object sender, EventArgs e)
        {
            System.Windows.Forms.DialogResult res = openFile.ShowDialog();
            OwnerEdit.ShowPopup();
            if (res == System.Windows.Forms.DialogResult.OK)
            {
                LoadImage(openFile.FileName);
                Invalidate();
            }
        }
        protected override void LayoutChanged()
        {
            base.LayoutChanged();
            ChangeButtonsPositions();
        }

        public virtual bool LoadImage(string fileName)
        {
            if (fileName != null && fileName != string.Empty )
            {
                try { myImage = Image.FromFile(fileName); }
                catch
                {
                    resultFileName = string.Empty;
                    ClearImage();
                    return false;
                }
                this.resultFileName = fileName;
                return true;
            }
            resultFileName = string.Empty;
            ClearImage();
            return false;

        }

        public void ClearImage()
        {
            if (myImage != null)
            {
                myImage.Dispose();
                myImage = null;
            }
        }

        public override Size CalcFormSize(Size contentSize)
        {
            return base.CalcFormSize(formContentSize);
        }
        
        public override object ResultValue
        {
            get
            {
                return resultFileName;
            }
        }

        protected override void ClosePopup(PopupCloseMode closeMode)
        {
            ClearImage();
            base.ClosePopup(closeMode);
            resultFileName = "";
        }

        public override void ShowPopupForm()
        {
            string fileName = OwnerEdit.EditValue as string;
            LoadImage(fileName);
            base.ShowPopupForm();
        }

        protected override PopupBaseFormPainter CreatePainter()
        {
            return new CustomPopupBaseFormPainter();
        }
        protected override PopupBaseFormViewInfo CreateViewInfo()
        {
            return new CustomPopupBaseSizeableFormViewInfo(this);
        }
        protected new CustomPopupBaseSizeableFormViewInfo ViewInfo { get { return base.ViewInfo as CustomPopupBaseSizeableFormViewInfo; } }
        protected override void OnSizeChanged(EventArgs e)
        {
            formContentSize = ViewInfo.ContentRect.Size;
            base.OnSizeChanged(e);
        }

    }
    public class CustomPopupBaseFormPainter : PopupBaseSizeableFormPainter
    {
        public CustomPopupBaseFormPainter()
            : base()
        {

        }

        public override void Draw(PopupFormGraphicsInfoArgs info)
        {
            base.Draw(info);
            DrawImage(info);
        }

        private void DrawImage(PopupFormGraphicsInfoArgs info)
        {
            CustomPopupBaseSizeableFormViewInfo vi = info.ViewInfo as CustomPopupBaseSizeableFormViewInfo;
            CustomPopupBaseSizeableForm form = vi.Form as CustomPopupBaseSizeableForm;
            if (form.myImage != null)
                info.Graphics.DrawImage(form.myImage, vi.ImageRect);
        }
    }
    public class CustomPopupBaseSizeableFormViewInfo : PopupBaseSizeableFormViewInfo
    {
        Rectangle okButtonRect, openButtonRect, clearButtonRect, cancelButtonRect, imageRect;
        const int borderWidth = 2, buttonWidth = 40, buttonHeight = 18;
        Size buttonSize;

        public CustomPopupBaseSizeableFormViewInfo(PopupBaseForm form)
            : base(form)
        {
            buttonSize = new Size(buttonWidth, buttonHeight);
            imageRect = Rectangle.Empty;
            okButtonRect = Rectangle.Empty;
            openButtonRect = Rectangle.Empty;
            clearButtonRect = Rectangle.Empty;
            cancelButtonRect = Rectangle.Empty;
        }
        public virtual Rectangle OkButtonRect { get { return okButtonRect; } }
        public virtual Rectangle OpenButtonRect { get { return openButtonRect; } }
        public virtual Rectangle ClearButtonRect { get { return clearButtonRect; } }
        public virtual Rectangle CancelButtonRect { get { return cancelButtonRect; } }
        public virtual Rectangle ImageRect { get { return imageRect; } }

        public virtual Size ButtonSize { get { return buttonSize; } }
        protected override void CalcRects()
        {
            base.CalcRects();
            imageRect = ContentRect;

            okButtonRect = new Rectangle(imageRect.Left + borderWidth, imageRect.Bottom + borderWidth, ButtonSize.Width, ButtonSize.Height);
            cancelButtonRect = okButtonRect;
            cancelButtonRect.Offset(borderWidth * 2 + buttonSize.Width, 0);
            openButtonRect = cancelButtonRect;
            openButtonRect.Offset(borderWidth * 6 + buttonSize.Width, 0);
            clearButtonRect = openButtonRect;
            clearButtonRect.Offset(borderWidth * 2 + buttonSize.Width, 0);
        }
    }
}
