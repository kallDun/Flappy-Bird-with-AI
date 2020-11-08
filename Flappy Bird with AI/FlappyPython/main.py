import os
import neat
import pygame as pygame


local_dir = os.path.dirname(__file__)
config_file = os.path.join(local_dir, 'config-feedforward.txt')


def eval_genomes(genome, config):
    global WIN, gen
    win = WIN
    gen += 1

    genome.fitness = 0
    net = neat.nn.FeedForwardNetwork.create(genome, config)
    clock = pygame.time.Clock()
    isGame = True


    while isGame:
        clock.tick(30)
        net.fitness += 0.1
        output = net.activate(birdLocation, distanceToUpTube, distanceToDownTube)
        if output > 0.5 :
            Click()


    return WIN


def Click():
    print("click")


def run():
    config = neat.config.Config(neat.DefaultGenome, neat.DefaultReproduction,
                                neat.DefaultSpeciesSet, neat.DefaultStagnation,
                                config_file)
    population = neat.Population(config)

    winner = population.run(eval_genomes, 1)
