# coolt-neuralt-natverk
Unity 2021.3.22f1 LTS


need to setup virtual enviroment (venv) manually using https://www.python.org/downloads/release/python-3913/ (python 3.9 64bit)

in cmd: 

cd C:\...\coolt-neuralt-natverk

../..python39xx/Python.exe -m venv venv

then inside venv:
  pip3 install torch torchvision torchaudio

  pip install mlagents

  verify with: mlagents-learn --help

probably  need to downgrade protobuff with pip pip install protobuf==3.20


simple tutorial: https://www.youtube.com/watch?v=zPFU30tbyKs&ab_channel=CodeMonkey

@article{juliani2020,
  title={Unity: A general platform for intelligent agents},
  author={Juliani, Arthur and Berges, Vincent-Pierre and Teng, Ervin and Cohen, Andrew and Harper, Jonathan and Elion, Chris and Goy, Chris and Gao, Yuan and Henry, Hunter and Mattar, Marwan and Lange, Danny},
  journal={arXiv preprint arXiv:1809.02627},
  year={2020}
}


@article{cohen2022,
  title={On the Use and Misuse of Abosrbing States in Multi-agent Reinforcement Learning},
  author={Cohen, Andrew and Teng, Ervin and Berges, Vincent-Pierre and Dong, Ruo-Ping and Henry, Hunter and Mattar, Marwan and Zook, Alexander and Ganguly, Sujoy},
  journal={RL in Games Workshop AAAI 2022},
  year={2022}
}
