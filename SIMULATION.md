# Simulation

## Logic

Simulation is done using synchronous sequential logic with clock signal
to simulate state of all elements every clock cycle.

Logic is first compiled to optimized data structures
and then can be run in step-by-step mode or in perdiodic clock cycles
mode simulation with defined time interval in milliseconds. Each cycle
all states are calculated. Simulation time is virtual and equal to
simulation cycle interval times current cycle number.

### Digital logic elements

- Clock (simple cycle based clock)
- Point (logic wire connector, element without state)
- Line (logic wire, use line start and/or end ellipse arrow to invert)
- Signal (input and output signal)
- Input (input signal)
- Output (output signal)
- And (AND gate, use line start and/or end ellipse arrow to for Nand gate)
- Or (OR gate, use line start and/or end ellipse arrow to for Nor gate)
- Xor (XOR gate, use line start and/or end ellipse arrow to for Xnor gate)
- Inverter (logic signal inverter)
- Timer-On (ON delay timer)
- Timer-Off (OFF delay timer)
- Timer-Pulse (PULSE timer)

### State

Gates, timers and signals have state that is 3-state boolean ( true/false/null).

### Timer-On logic

On Timer waits a Delay after an input goes high before turning the State on.  
The State goes low when the input goes low.

### Timer-Off logic

Off Timer causes State to go high when an input goes high 
and keeps it high for a Delay after the input goes low.

### Timer-Pulse logic

Pulse Timer causes State to go high when an input goes high 
and keeps it high for a Delay and then goes low.

### References

- http://en.wikipedia.org/wiki/Three-valued_logic
- http://en.wikipedia.org/wiki/Many-valued_logic
- http://en.wikipedia.org/wiki/Digital_circuit
- http://en.wikipedia.org/wiki/Synchronous_logic
- http://en.wikipedia.org/wiki/Sequential_logic
- http://en.wikipedia.org/wiki/Logic_gate
- http://en.wikipedia.org/wiki/Flip-flop_(electronics)
- http://en.wikipedia.org/wiki/Clock_signal
- http://en.wikipedia.org/wiki/State_(computer_science)
- http://en.wikipedia.org/wiki/Logic_design
- http://en.wikipedia.org/wiki/Logic_simulation