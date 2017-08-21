using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace stonerkart
{
    class LoginScreen : Screen
    {
        private InputBox username;
        private InputBox password;

        private Square usernameLabel;
        private Square passwordLabel;

        private Button loginButton;
        private Button registerButton;

        public LoginScreen() : base(new Imege(Textures.bg3))
        {
            username = new InputBox(1000, 100);
            addElement(username);
            username.moveTo(MoveTo.Center, 200);
            username.Border = new SolidBorder(3, Color.Black);
            username.Backcolor = Color.DimGray;

            password = new InputBox(1000, 100);
            addElement(password);
            password.moveTo(MoveTo.Center, 400);
            password.Border = new SolidBorder(3, Color.Black);
            password.Backcolor = Color.DimGray;

            usernameLabel = new Square(220, 60);
            addElement(usernameLabel);
            usernameLabel.moveTo(MoveTo.Center, 140);
            usernameLabel.Text = "Username";
            usernameLabel.textColor = Color.White;

            passwordLabel = new Square(220, 60);
            addElement(passwordLabel);
            passwordLabel.moveTo(MoveTo.Center, 340);
            passwordLabel.Text = "Password";
            passwordLabel.textColor = Color.White;

            loginButton = new Button(500, 90);
            loginButton.Backimege = new MemeImege(Textures.buttonbg2);
            addElement(loginButton);
            loginButton.moveTo(MoveTo.Center, 550);
            var logintext = new Square(250, 80);
            logintext.Text = "Log in";
            logintext.hoverable = false;
            loginButton.addChild(logintext);
            logintext.moveTo(MoveTo.Center, 0);
            loginButton.Border = new AnimatedBorder(Textures.border0, 5);
            loginButton.clicked += a => Controller.attemptLogin(username.Text, password.Text);

            registerButton = new Button(500, 90);
            registerButton.Backimege = new MemeImege(Textures.buttonbg2);
            addElement(registerButton);
            registerButton.moveTo(MoveTo.Center, 850);
            var registertext = new Square(500, 80);
            registertext.Text = "Register";
            registertext.hoverable = false;
            registerButton.addChild(registertext);
            registertext.moveTo(MoveTo.Center, 0);
            registerButton.Border = new AnimatedBorder(Textures.border0, 5);
        }
    }
}
