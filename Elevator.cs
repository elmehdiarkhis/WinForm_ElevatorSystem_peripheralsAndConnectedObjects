using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Elevator
{
    public partial class Elevator : UserControl
    {
        private bool[] etageStatus = { false, false, false };

        public bool isOpen = false;
        public bool isMooving = false;



        //Delegate + event==================================================================
        public delegate void ElevatorDelegate(object obj, ElevatorEventArgs e);

        // les event : apres Ouverture ou Fermeture complete de la porte
        public event ElevatorDelegate DoorIsOpen;
        public event ElevatorDelegate DoorIsClosed;
        //=========================================================================



        /// Delegate + event===========================================================
        public delegate void EtageChangedDelegate(object sender, EtageEventArgs e);
        // events
        public event EtageChangedDelegate isIn_Rc;
        public event EtageChangedDelegate isIn_E1;
        public event EtageChangedDelegate isIn_E2;
        //==============================================================================


        //variable Global====================
        private string operationEnCours;

        // timer pour l'animation
        private Timer time;
        //===================================


        public Elevator()
        {
            InitializeComponent();

            etageStatus[0] = true;


            //Click Event Listner ==============

            //OpenDoor
            buttonOpen.Click += Button_Click;
            //CloseDoor
            buttonClose.Click += Button_Click;

            //demande de mouvement
            button1E.Click += etageRequest_Click;
            button2E.Click += etageRequest_Click;
            buttonRC.Click += etageRequest_Click;
            buttonUpRc.Click += etageRequest_Click;
            buttonDown2E.Click += etageRequest_Click;
            buttonUp1E.Click += etageRequest_Click;



            //================================

            //Timer Setup======
            time = new Timer();
            time.Interval = 50; //50 milisecondes entre chaque tick
            time.Tick += Time_Tick; // Apres chaque 50 milisecondes, le timer va appeler la fonction Time_Tick
            time.Enabled = false;
            //=================
        }

        private void etageRequest_Click(object sender, EventArgs e)
        {
            Button btn = (Button)sender;

            if (isMooving == false)
            {
                if (btn.Text == "1E")
                {
                    if (etageStatus[0] == true)
                    {
                        operationEnCours = "Monter de RC vers E1";

                    }
                    else if (etageStatus[1] == true)
                    {
                        operationEnCours = "Ouvrir porte";

                    }
                    else if (etageStatus[2] == true)
                    {
                        operationEnCours = "Descendre de E2 vers E1";

                    }
                }
                else if (btn.Text == "2E" || btn.Name == "buttonDown2E")
                {
                    if (etageStatus[0] == true)
                    {
                        operationEnCours = "Monter de RC vers E2";

                    }
                    else if (etageStatus[1] == true)
                    {
                        operationEnCours = "Monter de E1 vers E2";

                    }
                    else if (etageStatus[2] == true)
                    {
                        operationEnCours = "Ouvrir porte";

                    }

                }
                else if (btn.Text == "RC" || btn.Name == "buttonUpRc")
                {
                    if (etageStatus[2] == true)
                    {
                        operationEnCours = "Descendre de E2 vers RC";

                    }
                    else if (etageStatus[1] == true)
                    {
                        operationEnCours = "Descendre de E1 vers RC";

                    }
                    else if (etageStatus[0] == true)
                    {
                        operationEnCours = "Ouvrir porte";

                    }
                }

                //activer le timer
                time.Enabled = true;
                time.Start();
                //====
            }

        }

        private void buttonUp1E_Click(object sender, EventArgs e)
        {
            if (isMooving == false)
            {
                if (etageStatus[0] == true)
                {
                    operationEnCours = "Monter de RC vers E1";

                    //activer le timer
                    time.Enabled = true;
                    time.Start();
                    //====
                }
            }
               
        }

        private void buttonDown1E_Click(object sender, EventArgs e)
        {
            if (isMooving == false)
            {
                if (etageStatus[2] == true)
                {
                    operationEnCours = "Descendre de E2 vers E1";
                    //activer le timer
                    time.Enabled = true;
                    time.Start();
                    //====
                }
            }        
        }



        //========================================================================================================================
        private void Button_Click(object sender, EventArgs e)
        {
            Button btn = (Button)sender;

            if (isMooving == false)
            {
                if (btn.Text == "Open")
                {
                    operationEnCours = "Ouvrir porte"; //l'operation en cours est > OuvrirP

                    //activer le timer
                    //ceci va permettre de > call la fonciton Time_Tick apres 50ms > d'appeler la fonction OuvrirPorte
                    time.Enabled = true;
                    time.Start();
                    //====

                }
                else if (btn.Text == "Close")
                {
                    operationEnCours = "Fermer porte"; //l'operation en cours est > OuvrirP

                    //activer le timer
                    //ceci va permettre de > call la fonciton Time_Tick apres 50ms > d'appeler la fonction OuvrirPorte
                    time.Enabled = true;
                    time.Start();
                    //====
                }
            }
               
        }
        //==================================================================================================================


        private void OuvrirPorte()
        {
            if (panelPorte.Width > 10) //si la porte n'est encore PAS ouverte completement
            {
                panelPorte.Width -= 4; //ouvrir un peux la porte,pour donner l'effet animation
            }
            else //si la porte est ouverte completement
            {
                //Desactiver le timer
                time.Stop();
                time.Enabled = false;
                //===

                isOpen = true;


                //lancer l'event "DoorIsOpen"
                if (DoorIsOpen != null)
                {
                    DoorIsOpen(this, new ElevatorEventArgs("la porte est ouverte"));
                }
                //===
            }
        }

        private void FermerPorte()
        {
            if (panelPorte.Width < 75) // si la porte n'est encore PAS fermer completement
            {
                panelPorte.Width += 4; //fermer un peux la porte,pour donner l'effet animation
            }
            else //si la porte est fermer completement
            {
                //Desactiver le timer
                time.Stop();
                time.Enabled = false;
                //===

                isOpen = false;

                //lancer l'event "DoorIsClosed"
                if (DoorIsClosed != null)
                {
                    DoorIsClosed(this, new ElevatorEventArgs("la porte est fermer"));
                }
                //===
            }
        }


        //============================ fonction qui sera appelée par le timer apres chaque 50 milisecondes==================
        private void Time_Tick(object sender, EventArgs e)
        {
            if (operationEnCours == "Ouvrir porte")
            {
                OuvrirPorte();
            }
            else if (operationEnCours == "Fermer porte")
            {
                FermerPorte();
            }
            else if (operationEnCours == "Monter de RC vers E1")
            {
                //ENTRE DANS CE IF TANT QUE LA PORTE NE C PAS FERMER COMPLETEMENT==========================================================
                if (panelPorte.Width < 75 && isOpen == true) // si la porte n'est encore PAS fermer completement
                {
                    panelPorte.Width += 4; //fermer un peux la porte,pour donner l'effet animation      
                }
                //LA PORTE A ETE FERMER COMPLETEMENT=====================================================================================
                else
                {
                    isOpen = false;

                    //lancer l'event "DoorIsClosed"
                    if (DoorIsClosed != null)
                    {
                        DoorIsClosed(this, new ElevatorEventArgs("la porte est fermer"));
                    }
                    //===

                    //FAIRE BOUGE L'elevator vers detisnation=========================================================================
                    if (panelCage.Location.Y > 97)
                    {
                        isMooving = true;
                        panelCage.Location = new Point(panelCage.Location.X, panelCage.Location.Y - 4);
                    }
                    //ARRIVER A DESTIONATION==========================================================================================
                    else
                    {
                        labelRC.BackColor = Color.Gray;
                        label1E.BackColor = Color.Green;
                        label2E.BackColor = Color.Gray;

                        //OUVRIRE LA PORTE==============================================================================
                        if (panelPorte.Width > 10 && isOpen == false) //si la porte n'est encore PAS ouverte completement
                        {
                            panelPorte.Width -= 4; //ouvrir un peux la porte,pour donner l'effet animation
                        }
                        //LA PORTE A ETE OUVERTE COMPLETEMENT============================================================
                        else
                        {
                            isOpen = true;
                            isMooving = false;

                            //lancer l'event "DoorIsOpen"
                            if (DoorIsOpen != null)
                            {
                                DoorIsOpen(this, new ElevatorEventArgs("la porte est ouverte"));
                            }
                            //===

                            //lancer l'event "isIn_E1"
                            if (isIn_E1 != null)
                            {
                                isIn_E1(this, new EtageEventArgs("Arriver a l'etage 1"));
                            }
                            //===

                            etageStatus[0] = false;
                            etageStatus[1] = true;
                            etageStatus[2] = false;
                            


                            //Desactiver le timer
                            time.Stop();
                            time.Enabled = false;
                            //===
                        }
                        //===============================================================================================

                    }
                    //===================================================================================================

                }
                //=========================================================================================================================

            }
            else if (operationEnCours == "Monter de RC vers E2")
            {
                //ENTRE DANS CE IF TANT QUE LA PORTE NE C PAS FERMER COMPLETEMENT==========================================================
                if (panelPorte.Width < 75 && isOpen == true) // si la porte n'est encore PAS fermer completement
                {
                    panelPorte.Width += 4; //fermer un peux la porte,pour donner l'effet animation

                }
                //LA PORTE A ETE FERMER COMPLETEMENT=====================================================================================
                else
                {
                    isOpen = false;

                    //lancer l'event "DoorIsClosed"
                    if (DoorIsClosed != null)
                    {
                        DoorIsClosed(this, new ElevatorEventArgs("la porte est fermer"));
                    }
                    //===

                    //FAIRE BOUGE L'elevator vers detisnation=========================================================================
                    if (panelCage.Location.Y > 40)
                    {
                        isMooving = true;
                        panelCage.Location = new Point(panelCage.Location.X, panelCage.Location.Y - 4);

                        if (panelCage.Location.Y < 120)
                        {
                            labelRC.BackColor = Color.Gray;
                            label1E.BackColor = Color.Green;
                            label2E.BackColor = Color.Gray;
                        }
                    }
                    //ARRIVER A DESTIONATION==========================================================================================
                    else
                    {
                        labelRC.BackColor = Color.Gray;
                        label1E.BackColor = Color.Gray;
                        label2E.BackColor = Color.Green;

                        //OUVRIR LA PORTE==============================================================================
                        if (panelPorte.Width > 10 && isOpen == false) //si la porte n'est encore PAS ouverte completement
                        {
                            panelPorte.Width -= 4; //ouvrir un peux la porte,pour donner l'effet animation
                        }
                        //LA PORTE A ETE OUVERTE COMPLETEMENT============================================================
                        else
                        {
                            isOpen = true;
                            isMooving = false;

                            //lancer l'event "DoorIsOpen"
                            if (DoorIsOpen != null)
                            {
                                DoorIsOpen(this, new ElevatorEventArgs("la porte est ouverte"));
                            }
                            //===

                            //lancer l'event "isIn_E1"
                            if (isIn_E1 != null)
                            {
                                isIn_E1(this, new EtageEventArgs("Arriver a l'etage 1"));
                            }
                            //===

                            etageStatus[0] = false;
                            etageStatus[1] = false;
                            etageStatus[2] = true;
                            


                            //Desactiver le timer
                            time.Stop();
                            time.Enabled = false;
                            //===
                        }
                        //===============================================================================================

                    }
                    //===================================================================================================

                }
                //=========================================================================================================================
            }
            else if (operationEnCours == "Descendre de E1 vers RC")
            {
                //ENTRE DANS CE IF TANT QUE LA PORTE NE C PAS FERMER COMPLETEMENT==========================================================
                if (panelPorte.Width < 75 && isOpen == true) // si la porte n'est encore PAS fermer completement
                {
                    panelPorte.Width += 4; //fermer un peux la porte,pour donner l'effet animation      
                }
                //LA PORTE A ETE FERMER COMPLETEMENT=====================================================================================
                else
                {
                    isOpen = false;

                    //lancer l'event "DoorIsClosed"
                    if (DoorIsClosed != null)
                    {
                        DoorIsClosed(this, new ElevatorEventArgs("la porte est fermer"));
                    }
                    //===

                    //FAIRE BOUGE L'elevator vers detisnation=========================================================================
                    if (panelCage.Location.Y < 150)
                    {
                        isMooving = true;
                        panelCage.Location = new Point(panelCage.Location.X, panelCage.Location.Y + 4);
                    }
                    //ARRIVER A DESTIONATION==========================================================================================
                    else
                    {

                        labelRC.BackColor = Color.Green;
                        label1E.BackColor = Color.Gray;
                        label2E.BackColor = Color.Gray;

                        //OUVRIR LA PORTE==============================================================================
                        if (panelPorte.Width > 10 && isOpen == false) //si la porte n'est encore PAS ouverte completement
                        {
                            panelPorte.Width -= 4; //ouvrir un peux la porte,pour donner l'effet animation
                        }
                        //LA PORTE A ETE OUVERTE COMPLETEMENT============================================================
                        else
                        {
                            isOpen = true;
                            isMooving = false;

                            //lancer l'event "DoorIsOpen"
                            if (DoorIsOpen != null)
                            {
                                DoorIsOpen(this, new ElevatorEventArgs("la porte est ouverte"));
                            }
                            //===

                            //lancer l'event "isIn_E1"
                            if (isIn_E1 != null)
                            {
                                isIn_E1(this, new EtageEventArgs("Arriver a l'etage 1"));
                            }
                            //===

                            etageStatus[0] = true;
                            etageStatus[1] = false;
                            etageStatus[2] = false;
                            



                            //Desactiver le timer
                            time.Stop();
                            time.Enabled = false;
                            //===
                        }
                        //===============================================================================================

                    }
                    //===================================================================================================

                }
                //=========================================================================================================================
            }
            else if (operationEnCours == "Descendre de E2 vers RC")
            {
                //ENTRE DANS CE IF TANT QUE LA PORTE NE C PAS FERMER COMPLETEMENT==========================================================
                if (panelPorte.Width < 75 && isOpen == true) // si la porte n'est encore PAS fermer completement
                {
                    panelPorte.Width += 4; //fermer un peux la porte,pour donner l'effet animation

                }
                //LA PORTE A ETE FERMER COMPLETEMENT=====================================================================================
                else
                {
                    isOpen = false;

                    //lancer l'event "DoorIsClosed"
                    if (DoorIsClosed != null)
                    {
                        DoorIsClosed(this, new ElevatorEventArgs("la porte est fermer"));
                    }
                    //===

                    //FAIRE BOUGE L'elevator vers detisnation=========================================================================
                    if (panelCage.Location.Y < 150)
                    {
                        isMooving = true;
                        panelCage.Location = new Point(panelCage.Location.X, panelCage.Location.Y + 4);

                        if (panelCage.Location.Y > 80)
                        {
                            labelRC.BackColor = Color.Gray;
                            label1E.BackColor = Color.Green;
                            label2E.BackColor = Color.Gray;
                        }
                    }
                    //ARRIVER A DESTIONATION==========================================================================================
                    else
                    {

                        labelRC.BackColor = Color.Green;
                        label1E.BackColor = Color.Gray;
                        label2E.BackColor = Color.Gray;

                        //OUVRIR LA PORTE==============================================================================
                        if (panelPorte.Width > 10 && isOpen == false) //si la porte n'est encore PAS ouverte completement
                        {
                            panelPorte.Width -= 4; //ouvrir un peux la porte,pour donner l'effet animation
                        }
                        //LA PORTE A ETE OUVERTE COMPLETEMENT============================================================
                        else
                        {
                            isOpen = true;
                            isMooving = false;

                            //lancer l'event "DoorIsOpen"
                            if (DoorIsOpen != null)
                            {
                                DoorIsOpen(this, new ElevatorEventArgs("la porte est ouverte"));
                            }
                            //===

                            //lancer l'event "isIn_E1"
                            if (isIn_E1 != null)
                            {
                                isIn_E1(this, new EtageEventArgs("Arriver a l'etage 1"));
                            }
                            //===

                            etageStatus[0] = true;
                            etageStatus[1] = false;
                            etageStatus[2] = false;
                           


                            //Desactiver le timer
                            time.Stop();
                            time.Enabled = false;
                            //===
                        }
                        //===============================================================================================

                    }
                    //===================================================================================================

                }
                //=========================================================================================================================
            }
            else if (operationEnCours == "Descendre de E2 vers E1")
            {
                //ENTRE DANS CE IF TANT QUE LA PORTE NE C PAS FERMER COMPLETEMENT==========================================================
                if (panelPorte.Width < 75 && isOpen == true) // si la porte n'est encore PAS fermer completement
                {
                    panelPorte.Width += 4; //fermer un peux la porte,pour donner l'effet animation      
                }
                //LA PORTE A ETE FERMER COMPLETEMENT=====================================================================================
                else
                {
                    isOpen = false;

                    //lancer l'event "DoorIsClosed"
                    if (DoorIsClosed != null)
                    {
                        DoorIsClosed(this, new ElevatorEventArgs("la porte est fermer"));
                    }
                    //===

                    //FAIRE BOUGE L'elevator vers detisnation=========================================================================
                    if (panelCage.Location.Y < 90)
                    {
                        isMooving = true;
                        panelCage.Location = new Point(panelCage.Location.X, panelCage.Location.Y + 4);
                    }
                    //ARRIVER A DESTIONATION==========================================================================================
                    else
                    {

                        labelRC.BackColor = Color.Gray;
                        label1E.BackColor = Color.Green;
                        label2E.BackColor = Color.Gray;

                        //OUVRIR LA PORTE==============================================================================
                        if (panelPorte.Width > 10 && isOpen == false) //si la porte n'est encore PAS ouverte completement
                        {
                            panelPorte.Width -= 4; //ouvrir un peux la porte,pour donner l'effet animation
                        }
                        //LA PORTE A ETE OUVERTE COMPLETEMENT============================================================
                        else
                        {
                            isOpen = true;
                            isMooving = false;

                            //lancer l'event "DoorIsOpen"
                            if (DoorIsOpen != null)
                            {
                                DoorIsOpen(this, new ElevatorEventArgs("la porte est ouverte"));
                            }
                            //===

                            //lancer l'event "isIn_E1"
                            if (isIn_E1 != null)
                            {
                                isIn_E1(this, new EtageEventArgs("Arriver a l'etage 1"));
                            }
                            //===

                            etageStatus[0] = false;
                            etageStatus[1] = true;
                            etageStatus[2] = false;
                            


                            //Desactiver le timer
                            time.Stop();
                            time.Enabled = false;
                            //===
                        }
                        //===============================================================================================

                    }
                    //===================================================================================================

                }
                //=========================================================================================================================
            }
            else if (operationEnCours == "Monter de E1 vers E2")
            {
                //ENTRE DANS CE IF TANT QUE LA PORTE NE C PAS FERMER COMPLETEMENT==========================================================
                if (panelPorte.Width < 75 && isOpen == true) // si la porte n'est encore PAS fermer completement
                {
                    panelPorte.Width += 4; //fermer un peux la porte,pour donner l'effet animation      
                }
                //LA PORTE A ETE FERMER COMPLETEMENT=====================================================================================
                else
                {
                    isOpen = false;

                    //lancer l'event "DoorIsClosed"
                    if (DoorIsClosed != null)
                    {
                        DoorIsClosed(this, new ElevatorEventArgs("la porte est fermer"));
                    }
                    //===

                    //FAIRE BOUGE L'elevator vers detisnation=========================================================================
                    if (panelCage.Location.Y > 40)
                    {
                        isMooving = true;
                        panelCage.Location = new Point(panelCage.Location.X, panelCage.Location.Y - 4);
                    }
                    //ARRIVER A DESTIONATION==========================================================================================
                    else
                    {

                        labelRC.BackColor = Color.Gray;
                        label1E.BackColor = Color.Gray;
                        label2E.BackColor = Color.Green;

                        //OUVRIR LA PORTE==============================================================================
                        if (panelPorte.Width > 10 && isOpen == false) //si la porte n'est encore PAS ouverte completement
                        {
                            panelPorte.Width -= 4; //ouvrir un peux la porte,pour donner l'effet animation
                        }
                        //LA PORTE A ETE OUVERTE COMPLETEMENT============================================================
                        else
                        {
                            isOpen = true;
                            isMooving = false;

                            //lancer l'event "DoorIsOpen"
                            if (DoorIsOpen != null)
                            {
                                DoorIsOpen(this, new ElevatorEventArgs("la porte est ouverte"));
                            }
                            //===

                            //lancer l'event "isIn_E1"
                            if (isIn_E1 != null)
                            {
                                isIn_E1(this, new EtageEventArgs("Arriver a l'etage 1"));
                            }
                            //===
                            
                            etageStatus[0] = false;
                            etageStatus[1] = false;
                            etageStatus[2] = true;
                            


                            //Desactiver le timer
                            time.Stop();
                            time.Enabled = false;
                            //===
                        }
                        //===============================================================================================

                    }
                    //===================================================================================================

                }
                //=========================================================================================================================

            }
            //==================================================================================================================

        }




        

        //=======================================================================================================================================
        [Browsable(true)] // attribut pour permettre de changer le temps d'animation depuis PROPRIETE===
        public int AnimTime
        {
            get { return this.time.Interval; }
            set { this.time.Interval = (value > 0) ? value : 200; }
        }
        //=======================================================================================================================================















        private void UserControl1_Load(object sender, EventArgs e)
        {

        }
        private void buttonOpen_Click(object sender, EventArgs e)
        {

        }
        private void buttonClose_Click(object sender, EventArgs e)
        {

        }
        private void Lum_Rc_On()
        {

        }
        private void Lum_Rc_Off()
        {

        }
        private void Lum_1E_On()
        {

        }
        private void Lum_1E_Off()
        {

        }

        private void Lum_2E_On()
        {

        }
        private void Lum_2E_Off()
        {

        }


        private void panel6_Paint(object sender, PaintEventArgs e)
        {

        }

        private void panel7_Paint(object sender, PaintEventArgs e)
        {

        }

        private void panel1_Paint(object sender, PaintEventArgs e)
        {

        }

        
    }

        
   }
