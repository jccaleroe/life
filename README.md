## Artificial Life Project

#### Project made with Unity 2018.1.0f2
##### Juan Camilo Calero Espinosa

This project is part of the Artificial Life course at the [National University of Colombia](http://unal.edu.co/), taught by [Jonatan Gómez](http://dis.unal.edu.co/~jgomezpe/). 

A video of the project can be found on [YouTube](https://www.youtube.com/watch?v=WD_qkvjUTC0)

### Description

This project consists of two species, Fishes and Whales, Fishes
search for food moving with a Boids Algorithm. This Algorithm
considers different points of space as attractors to each single
fish. Those point are the center of each fish's neighborhood,
an avoidance point to stay a little saparate from other fishes, 
an energy point, which is the best food that a fish can see 
(each fish has a vision radius) according to 
a balance of distance, energy that it has and contamination on 
that point (each time a fish eats, it produces some contamination) and 
a scape point which is activated when a whale is close, this attractor
is the strongest but is only activated when the fish feels is in danger.
When a Fish is too close to a energy source (energy sources are fractal 
binary trees, made with a Lindenmayer System with a recursion limit 
of 3) it stops to it's minimum speed, but if it is part of a flock, 
the fish prefers to move with the average speed of the group. If the 
fish is searching for food or is scaping from a whale, it swims with 
it's fastest speed.

Each fish and whale have a metabolism rate, which tells the amount of 
energy they lose at each second as well as an age variable that 
increases per second. Some of those two variables will trigger the 
agent's death.

Fishes are always searching for food, but whales stop when they have 
plenty of energy. 

Whales chase the closest fish, but if two are almost
the same as far, it prefers the one he was hunting.

Fishes can have children, rewarding the ones that can survive longer,
for this, male and female have to like each other according to their 
phenotype (the phenotype is the non-linear affine tranformation and skin
texture (generated with turing-morph when the fish is born)) but from 
time on time they ignore this discrimination.
It they agree to reproduce, a cross-over and mutation is applied to 
the variables of the parents and are inherited to the offspring, those 
variables are the energy it can take from a tree on a single bite, 
vision, expectedLife, capacity, metabolism, reproductionRate, 
affine transformation, alpha paramater in turing-morph and starting 
energy.

Whales can reproduce too, for this, there are reproduction seasons and
females choose the male with the biggest amount of energy, then females 
approach to the male and when are close enough they reproduce.
The variables that are inherited are the vision, expectedLife, capacity,
metabolism, and starting energy. When a female is reproduced, she has to
wait some time no matter if she is in a reproduction season.

Trees are generated on different points of space, but only some of those
sources are activated from time on time, this allows to see how fishes 
migrate using boids. 

When a fish or whale is born, they have to wait some time before they
can reproduce, and when they reproduce, they'll have to wait another 
time (the reproduction rate) to be able to do that again.   
  
To make the movement of the agents smoother, they don't change their
direction at each frame, instead a Levy Walk is implemented so they
have a probability of update their direction and speed at each frame.

To move the agents, a Rigidbody and physics were used (thre is no 
gravity), so fishes move differenly according to their affine 
tranformation and size, also the smaller the fish, the fewer the chance
to be caught by a whale, but also the fewer the chances to eat.
